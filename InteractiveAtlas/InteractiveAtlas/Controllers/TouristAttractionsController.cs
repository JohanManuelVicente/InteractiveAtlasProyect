using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Entities;
using System.Xml.Linq;
using InteractiveAtlas.Data;
using InteractiveAtlas.DTOs;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TouristAttractionsController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;

        public TouristAttractionsController(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTouristAttractions()
        {
            var touristAttractions = _context.TouristAttractions.ToList();
            return Ok(touristAttractions);
        }

        [HttpGet("{id}")]
        public IActionResult GetTouristAttractionById(int id)
        {
            var touristAttraction = _context.TouristAttractions.FirstOrDefault(t => t.Id == id);

            if (touristAttraction == null)
            {
                return BadRequest($"Tourist Attraction with ID: {id} not found");
            }

            return Ok(touristAttraction);
        }

        [HttpPost]
        public IActionResult CreateTouristAttraction([FromBody] TouristAttractionDto request)
        {
            if (request == null)
            {
                return BadRequest("The TouristAttraction cannot be null");
            }

            var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId);
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
                ProvinceId = request.ProvinceId,
            };

            _context.TouristAttractions.Add(touristAttraction);
            _context.SaveChanges();
            return Ok(new { id = touristAttraction.Id });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTouristAttraction(int id, [FromBody] TouristAttractionDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                return BadRequest("El nombre de la atracción turística es nulo");
            }

            var existingTouristAttraction = _context.TouristAttractions.FirstOrDefault(ta => ta.Id == request.Id);
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

            _context.TouristAttractions.Update(existingTouristAttraction);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTouristAttraction(int id)
        {
            var touristAttraction = _context.TouristAttractions.FirstOrDefault(t => t.Id == id);
            if (touristAttraction == null)
            {
                return NotFound($"TouristAttraction con ID: {id} no fue encontrada");
            }

            _context.TouristAttractions.Remove(touristAttraction);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
