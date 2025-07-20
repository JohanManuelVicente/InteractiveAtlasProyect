using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.DTOs;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using InteractiveAtlas.Infrastucture;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {

        private readonly InteractiveAtlasDbContext _context;

        public ProvincesController(InteractiveAtlasDbContext context)
        {

            _context = context;

        }


        [HttpGet]

        public IActionResult GetProvinces()
        {
           var provinces = _context.Provinces.ToList();

            var provincesResponse = new List<ProvinceDto>();

            /* Forma mas funcional, solo que menos actualizada
            foreach (var p in provinces)
            {
                var provinceDto = new ProvinceDto()
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
                };

            }*/

            // Forma usando Linq, mas moderna pero dificil de debbugear
            provincesResponse = provinces.Select(p => new ProvinceDto
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


            return Ok(provincesResponse);
        }

        [HttpGet("{id}")]

        public IActionResult GetProvinceById(int id)
        {
            var province = _context.Provinces.FirstOrDefault(province => province.Id == id);
            //province = _provinces.Where(province => province.Id == id).FirstOrDefault();

            /*    var province = _context.Provinces
            //.Include(p => p.TouristAttractions)
            //.Include(p => p.TypicalProducts)
            //.Include(p => p.QuizQuestions)
            //.FirstOrDefault(p => p.Id == id);
            */
            
            if (province == null)
            {
                return BadRequest($"Province with ID: {id} not found");
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
            return Ok(provinceResponse);

            /*var province = _context.Provinces
    .FirstOrDefault(p => p.Id == id);

            if (province == null)
                return NotFound($"Provincia con ID {id} no encontrada");

            var provinceDto = new ProvinceDto
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

            return Ok(provinceDto);
            */
        }

        [HttpPost]

        public IActionResult CreateProvince([FromBody] ProvinceDto required)
        {
            if (required == null)
            {
                return BadRequest("The Province cannot be null");
            }
            var province = new Province
            {
                Name = required.Name,
                Capital = required.Capital,
                AreaKm2 = required.AreaKm2,
                Population = required.Population,
                Density = required.Density,
                Region = required.Region,
                Latitude = required.Latitude,
                Longitude = required.Longitude,
                ImageUrl = required.ImageUrl,
                Description = required.Description
            };
            _context.Provinces.Add(province);
            _context.SaveChanges();
            return Ok(new { id = province.Id });

        }

        [HttpPut("{id}")]

        public IActionResult UpdateProvince(int id, [FromBody] ProvinceDto required)
        {
            if (id != required.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (required.Name == null)
            {
                return BadRequest("El nombre de la provincia es nulo o ID no coincide");
            }

            var existingProvince = _context.Provinces.FirstOrDefault(prov => prov.Id == required.Id);
            if (existingProvince == null)
            {
                return NotFound($" La provincia con ID {required.Id} no fue encontrado");
            }

            if (required.Capital == null)
            {
                return BadRequest("La capital de la provincia es nulo o ID no coincide");
            }

            if (required.Region == null)
            {
                return BadRequest("La region de la provincia es nulo o ID no coincide");
            }

            existingProvince.Name = required.Name;
            existingProvince.Capital = required.Capital;
            existingProvince.AreaKm2    = required.AreaKm2;
            existingProvince.Population = required.Population;
            existingProvince.Density = required.Density;
            existingProvince.Region = required.Region;
            existingProvince.Latitude = required.Latitude;
            existingProvince.Longitude = required.Longitude;
            existingProvince.ImageUrl = required.ImageUrl;
            existingProvince.Description = required.Description;
            //foranea
            _context.Provinces.Update(existingProvince);
            _context.SaveChanges();
            return NoContent();


        }

        [HttpDelete("{id}")]

        public IActionResult DeleteProvince (int id)
        {
            var province = _context.Provinces.FirstOrDefault(p => p.Id == id);
            if (province == null)
            {
                return NotFound($"Provincia con ID: {id} no fue encontrada");
            }

            _context.Provinces.Remove(province);
            _context.SaveChanges();
            return NoContent();


        }
    }
}
