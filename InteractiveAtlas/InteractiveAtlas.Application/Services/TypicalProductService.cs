
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;

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

            var TypicalProduct = await _unitOfWork.TypicalProducts.GetAllAsync();

            returns;
        }

        public async Task<IActionResult> GetTypicalProductsWithProvince()
        {

            var typicalProducts = await _unitOfWork.TypicalProducts.GetAllTypicalProductWithProvinceAsync();


            var typicalProductsResponse = new List<TypicalProductDto>();
            typicalProductsResponse = typicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId,
                ProvinceName = t.Province.Name


            }).ToList();
            return Ok(typicalProductsResponse);
        }

        public async Task<IActionResult> GetTypicalProductsById(int id)
        {
            var typicalProduct = await _unitOfWork.TypicalProducts.GetTypicalProductWithProvinceByIdAsync(id);

            if (typicalProduct == null)
            { return BadRequest($"Typical Product with ID: {id} not found"); }

            var typicalProductResponse = new TypicalProductDto
            {
                Id = typicalProduct.Id,
                Name = typicalProduct.Name,
                Description = typicalProduct.Description,
                ImageUrl = typicalProduct.ImageUrl,
                ProvinceId = typicalProduct.ProvinceId
            };
            return Ok(typicalProductResponse);

        }

        public async Task<IActionResult> GettypicalProductsByProvinceId([FromQuery]int provinceId)
        {
            return Ok(await _unitOfWork.TypicalProducts.GetAllTypicalProductByProvinceIdAsync(provinceId));

        }


        public async Task<IActionResult> CreateTypicalProduct([FromBody] TypicalProductDto request)
        {
            if (request == null)
            {
                return BadRequest("The TypicalProduct cannot be null");
            }

            var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("El nombre del producto típico es requerido");
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

            return Ok(new { id = typicalProduct.Id });

        }

        public async Task<IActionResult> UpdateTypicalProduct(int id, [FromBody] TypicalProductDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                return BadRequest("El nombre del producto típico es nulo");
            }

              var existingTypicalProduct = await _unitOfWork.TypicalProducts.GetTypicalProductWithProvinceByIdAsync(id);
            if (existingTypicalProduct == null)
            {
                return NotFound($"El producto típico con ID {request.Id} no fue encontrado");
            }

            var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
            }

            existingTypicalProduct.Name = request.Name;
            existingTypicalProduct.Description = request.Description;
            existingTypicalProduct.ImageUrl = request.ImageUrl;
            existingTypicalProduct.ProvinceId = request.ProvinceId; 

            _unitOfWork.TypicalProducts.UpdateAsync(existingTypicalProduct).Wait();
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }


        public async Task<IActionResult> DeleteTypicalProduct(int id)
        {
            var typicalProduct = await _unitOfWork.TypicalProducts.GetByIdAsync(id);
            if (typicalProduct == null)
            {
                return NotFound($"TypicalProduct con ID: {id} no fue encontrada");
            }

            await _unitOfWork.TypicalProducts.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();


        }
    }
}
