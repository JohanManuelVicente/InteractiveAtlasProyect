
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Services
{
    public class ProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProvinceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProvinceDto>> GetProvinces()
        {
            var provinces = await _unitOfWork.Provinces.GetAllAsync();

            var provincesResponse = provinces.Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name,
                Capital = p.Capital,
                AreaKm2 = p.AreaKm2,
                Population = p.Population,
                Density = p.Density,
                Region = p.Region,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageUrl = p.ImageUrl,
                Description = p.Description
            }).ToList();

            return provincesResponse;
        }

        public async Task<List<ProvinceDto>> GetProvincesWithTypicalProducts()
        {
            var provinces = await _unitOfWork.Provinces.GetAllProvincesWithDetailsAsync();

            var provincesResponse = provinces.Select(p => new ProvinceDto
            {
                Id = p.Id,
                Name = p.Name,
                Capital = p.Capital,
                AreaKm2 = p.AreaKm2,
                Population = p.Population,
                Density = p.Density,
                Region = p.Region,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                TypicalProducts = p.TypicalProducts?.Select(tp => new TypicalProductDto
                {
                    Id = tp.Id,
                    Name = tp.Name,
                    Description = tp.Description,
                    ImageUrl = tp.ImageUrl,
                    ProvinceId = tp.ProvinceId
                }).ToList(),
                TouristAttractions = p.TouristAttractions?.Select(ta => new TouristAttractionDto
                {
                    Id = ta.Id,
                    Name = ta.Name,
                    Description = ta.Description,
                    ImageUrl = ta.ImageUrl,
                    ProvinceId = ta.ProvinceId
                }).ToList()

            }).ToList();

            return provincesResponse;
        }

        public async Task<ProvinceDto> GetProvinceById(int id)
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);

            if (province == null)
            {
                throw new Exception($"Province with ID: {id} not found");
            }

            var provinceResponse = new ProvinceDto
            {
                Id = province.Id,
                Name = province.Name,
                Capital = province.Capital,
                AreaKm2 = province.AreaKm2,
                Population = province.Population,
                Density = province.Density,
                Region = province.Region,
                Latitude = province.Latitude,
                Longitude = province.Longitude,
                ImageUrl = province.ImageUrl,
                Description = province.Description
            };

            return provinceResponse;
        }

        public async Task<int> CreateProvince(ProvinceDto request)
        {
            if (request == null)
            {
                throw new Exception("The Province cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new Exception("El nombre de la provincia es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Capital))
            {
                throw new Exception("La capital de la provincia es requerida");
            }

            if (string.IsNullOrWhiteSpace(request.Region))
            {
                throw new Exception("La región de la provincia es requerida");
            }

            var province = new Province
            {
                Name = request.Name,
                Capital = request.Capital,
                AreaKm2 = request.AreaKm2,
                Population = request.Population,
                Density = request.Density,
                Region = request.Region,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ImageUrl = request.ImageUrl,
                Description = request.Description
            };

            province = await _unitOfWork.Provinces.AddAsync(province);
            await _unitOfWork.CompleteAsync();

            return province.Id;
        }

        public async Task UpdateProvince(int id, ProvinceDto request)
        {
            if (id != request.Id)
            {
                throw new Exception("El ID de la URL no coincide con el ID de la petición");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new Exception("El nombre de la provincia es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Capital))
            {
                throw new Exception("La capital de la provincia es requerida");
            }

            if (string.IsNullOrWhiteSpace(request.Region))
            {
                throw new Exception("La región de la provincia es requerida");
            }

            var existingProvince = await _unitOfWork.Provinces.GetByIdAsync(id);
            if (existingProvince == null)
            {
                throw new Exception($"La provincia con ID {request.Id} no fue encontrada");
            }

            existingProvince.Name = request.Name;
            existingProvince.Capital = request.Capital;
            existingProvince.AreaKm2 = request.AreaKm2;
            existingProvince.Population = request.Population;
            existingProvince.Density = request.Density;
            existingProvince.Region = request.Region;
            existingProvince.Latitude = request.Latitude;
            existingProvince.Longitude = request.Longitude;
            existingProvince.ImageUrl = request.ImageUrl;
            existingProvince.Description = request.Description;

            await _unitOfWork.Provinces.UpdateAsync(existingProvince);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProvince(int id)
        {
            var province = await _unitOfWork.Provinces.GetByIdAsync(id);
            if (province == null)
            {
                throw new Exception($"Provincia con ID: {id} no fue encontrada");
            }

            await _unitOfWork.Provinces.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
