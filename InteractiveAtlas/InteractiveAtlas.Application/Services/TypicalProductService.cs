
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Services
{
  
    public class TypicalProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypicalProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    

        public async Task<List<TypicalProductDto>> GetTypicalProducts()
        {

            var TypicalProducts = await _unitOfWork.TypicalProducts.GetAllAsync();

            var TypicalProductsList = TypicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ProvinceId = t.ProvinceId,

            }).ToList();
            return TypicalProductsList;
        }

        public async Task<List<TypicalProductDto>> GetTypicalProductsWithProvince()
        {

            var TypicalProducts = await _unitOfWork.TypicalProducts.GetAllTypicalProductWithProvinceAsync();


            var TypicalProductsResponse = new List<TypicalProductDto>();
            TypicalProductsResponse = TypicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId,
                ProvinceName = t.Province.Name


            }).ToList();
            return TypicalProductsResponse;
        }

        public async Task<TypicalProductDto> GetTypicalProductsById(int id)
        {
            var typicalProduct = await _unitOfWork.TypicalProducts.GetTypicalProductWithProvinceByIdAsync(id);

            if (typicalProduct == null)
            {throw new Exception($"Typical Product with ID: {id} not found") ; }

            var typicalProductResponse = new TypicalProductDto
            {
                Id = typicalProduct.Id,
                Name = typicalProduct.Name,
                Description = typicalProduct.Description,
                ImageUrl = typicalProduct.ImageUrl,
                ProvinceId = typicalProduct.ProvinceId
            };
            return typicalProductResponse;

        }

        public async Task<List<TypicalProductDto>> GettypicalProductsByProvinceId(int provinceId)
        {
            var TypicalProducts = await _unitOfWork.TypicalProducts.GetAllTypicalProductByProvinceIdAsync(provinceId);

            var TypicalProductsList = TypicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ProvinceId = t.ProvinceId,

            }).ToList();


            return TypicalProductsList;

        }


        public async Task<int> CreateTypicalProduct( TypicalProductDto request)
        {
            if (request == null)
            {
                throw new Exception ("The TypicalProduct cannot be null");
            }

            var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                throw new Exception ( $"La provincia con ID {request.ProvinceId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new Exception("El nombre del producto típico es requerido");
            }

            var typicalProduct = new TypicalProduct
            {
                Name = request.Name,

                Description = request.Description,

                ImageUrl = request.ImageUrl,

                ProvinceId = request.ProvinceId,

            };
            typicalProduct = await _unitOfWork.TypicalProducts.AddAsync(typicalProduct);
            await _unitOfWork.CompleteAsync();

            return typicalProduct.Id;

        }

        public async Task UpdateTypicalProduct(int id, TypicalProductDto request)
        {
            if (id != request.Id)
            {
                throw new Exception ("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                throw new Exception ("El nombre del producto típico es nulo");
            }

              var existingTypicalProduct = await _unitOfWork.TypicalProducts.GetTypicalProductWithProvinceByIdAsync(id);
            if (existingTypicalProduct == null)
            {
                throw new Exception ($"El producto típico con ID {request.Id} no fue encontrado");
            }

            var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                throw new Exception ($"La provincia con ID {request.ProvinceId} no existe");
            }

            existingTypicalProduct.Name = request.Name;
            existingTypicalProduct.Description = request.Description;
            existingTypicalProduct.ImageUrl = request.ImageUrl;
            existingTypicalProduct.ProvinceId = request.ProvinceId; 

            _unitOfWork.TypicalProducts.UpdateAsync(existingTypicalProduct).Wait();
            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteTypicalProduct(int id)
        {
            var typicalProduct = await _unitOfWork.TypicalProducts.GetByIdAsync(id);
            if (typicalProduct == null)
            {
                throw new Exception ($"TypicalProduct con ID: {id} no fue encontrada");
            }

            await _unitOfWork.TypicalProducts.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
