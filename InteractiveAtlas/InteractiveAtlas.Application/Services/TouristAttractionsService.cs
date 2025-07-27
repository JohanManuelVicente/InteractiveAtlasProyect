
using InteractiveAtlas.Domain.Entities;

using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InteractiveAtlas.Services
{
    public class TouristAttractionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TouristAttractionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TouristAttractionDto>> GetTouristAttractions()
        {
            var touristAttractions = await _unitOfWork.TouristAttractions.GetAllAsync();

            return touristAttractions.Select(t => new TouristAttractionDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Location = t.Location,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId
            }).ToList();
        }

        public async Task<List<TouristAttractionDto>> GetTouristAttractionsWithProvince()
        {
            var touristAttractions = await _unitOfWork.TouristAttractions.GetAllTouristAttractionWithProvinceAsync();

            var touristAttractionsResponse = new List<TouristAttractionDto>();
            touristAttractionsResponse = touristAttractions.Select(t => new TouristAttractionDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Location = t.Location,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId,
                ProvinceName = t.Province.Name
            }).ToList();
            return touristAttractionsResponse;
        }

        public async Task<TouristAttractionDto> GetTouristAttractionById(int id)
        {
            var touristAttraction = await _unitOfWork.TouristAttractions.GetTouristAttractionWithProvinceByIdAsync(id);

            if (touristAttraction == null)
            {
                throw new Exception($"Tourist Attraction with ID: {id} not found");
            }

            var touristAttractionResponse = new TouristAttractionDto
            {
                Id = touristAttraction.Id,
                Name = touristAttraction.Name,
                Description = touristAttraction.Description,
                Location = touristAttraction.Location,
                ImageUrl = touristAttraction.ImageUrl,
                ProvinceId = touristAttraction.ProvinceId
            };
            return touristAttractionResponse;
        }

        public async Task<List<TouristAttractionDto>> GetTouristAttractionsByProvinceId(int provinceId)
        {
            var touristAttractions = await _unitOfWork.TouristAttractions.GetAllTouristAttractionByProvinceIdAsync(provinceId);

            return touristAttractions.Select(t => new TouristAttractionDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Location = t.Location,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId
            }).ToList();
        }

        public async Task<int> CreateTouristAttraction(TouristAttractionDto request)
        {
            if (request == null)
            {
                throw new Exception("The TouristAttraction cannot be null");
            }

            var provinceExists = await _unitOfWork.Context.Provinces.AnyAsync(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                throw new Exception($"La provincia con ID {request.ProvinceId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new Exception("El nombre de la atracción turística es requerido");
            }

            var touristAttraction = new TouristAttraction
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                ImageUrl = request.ImageUrl,
                ProvinceId = request.ProvinceId
            };

            touristAttraction = await _unitOfWork.TouristAttractions.AddAsync(touristAttraction);
            await _unitOfWork.CompleteAsync();

            return touristAttraction.Id;
        }

        public async Task UpdateTouristAttraction(int id, TouristAttractionDto request)
        {
            if (id != request.Id)
            {
                throw new Exception("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                throw new Exception("El nombre de la atracción turística es nulo");
            }

            var existingTouristAttraction = await _unitOfWork.TouristAttractions.GetTouristAttractionWithProvinceByIdAsync(id);
            if (existingTouristAttraction == null)
            {
                throw new Exception($"La atracción turística con ID {request.Id} no fue encontrada");
            }

            var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                throw new Exception($"La provincia con ID {request.ProvinceId} no existe");
            }

            existingTouristAttraction.Name = request.Name;
            existingTouristAttraction.Description = request.Description;
            existingTouristAttraction.Location = request.Location;
            existingTouristAttraction.ImageUrl = request.ImageUrl;
            existingTouristAttraction.ProvinceId = request.ProvinceId;

            _unitOfWork.TouristAttractions.UpdateAsync(existingTouristAttraction).Wait();
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteTouristAttraction(int id)
        {
            var touristAttraction = await _unitOfWork.TouristAttractions.GetByIdAsync(id);
            if (touristAttraction == null)
            {
                throw new Exception($"TouristAttraction con ID: {id} no fue encontrada");
            }

            await _unitOfWork.TouristAttractions.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
