using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.DTOs;
using InteractiveAtlas.Infrastucture;
using InteractiveAtlas.Infrastucture.Repository;
using Microsoft.EntityFrameworkCore;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TouristAttractionsController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;
        private readonly ProvinceRepository _provinceRepository;
        private readonly TouristAttractionRepository _touristAttractionRepository;

        public TouristAttractionsController(InteractiveAtlasDbContext context, ProvinceRepository provinceRepository, TouristAttractionRepository touristAttractionRepository)
        {
            _context = context;
            _provinceRepository = provinceRepository;
            _touristAttractionRepository = touristAttractionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTouristAttractions()
        {
            return Ok(await _touristAttractionRepository.GetAllTouristAttractionAsync());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetTouristAttractionsWithProvince()
        {
            var touristAttractions = await _touristAttractionRepository.GetAllTouristAttractionWithProvinceAsync();

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
            return Ok(touristAttractionsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTouristAttractionById(int id)
        {
            var touristAttraction = await _touristAttractionRepository.GetTouristAttractionByIdAsync(id);

            if (touristAttraction == null)
            {
                return BadRequest($"Tourist Attraction with ID: {id} not found");
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
            return Ok(touristAttractionResponse);
        }

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GetTouristAttractionsByProvinceId([FromQuery] int provinceId)
        {
            return Ok(await _touristAttractionRepository.GetAllTouristAttractionByProvinceIdAsync(provinceId));
        }

        [HttpPost]
        
        public async Task<IActionResult> CreateTouristAttraction([FromBody] TouristAttractionDto request)
        {
            if (request == null)
            {
                return BadRequest("The TouristAttraction cannot be null");
            }

            
            var provinceExists = await _context.Provinces.AnyAsync(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("El nombre de la atracción turística es requerido");
            }

            var touristAttraction = new TouristAttraction
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                ImageUrl = request.ImageUrl,
                ProvinceId = request.ProvinceId
               
            };

            touristAttraction = await _touristAttractionRepository.AddTouristAttractionAsync(touristAttraction);

            return Ok(new { id = touristAttraction.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTouristAttraction(int id, [FromBody] TouristAttractionDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                return BadRequest("El nombre de la atracción turística es nulo");
            }

            var existingTouristAttraction = await _touristAttractionRepository.GetTouristAttractionByIdAsync(id);
            if (existingTouristAttraction == null)
            {
                return NotFound($"La atracción turística con ID {request.Id} no fue encontrada");
            }

            var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
            }

            existingTouristAttraction.Name = request.Name;
            existingTouristAttraction.Description = request.Description;
            existingTouristAttraction.Location = request.Location;
            existingTouristAttraction.ImageUrl = request.ImageUrl;
            existingTouristAttraction.ProvinceId = request.ProvinceId;

            _touristAttractionRepository.UpdateTouristAttractionAsync(existingTouristAttraction).Wait();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTouristAttraction(int id)
        {
            var touristAttraction = await _touristAttractionRepository.GetTouristAttractionByIdAsync(id);
            if (touristAttraction == null)
            {
                return NotFound($"TouristAttraction con ID: {id} no fue encontrada");
            }

            await _touristAttractionRepository.DeleteTouristAttractionAsync(id);
            return NoContent();
        }
    }
}
