using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;

using InteractiveAtlas.DTOs;
using InteractiveAtlas.Infrastucture;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypicalProductsController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;

        public TypicalProductsController(InteractiveAtlasDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult GetTypicalProducts()
        {

            var typicalProducts = _context.TypicalProducts.ToList();
            var typicalProductsResponse = new List<TypicalProductDto>();
            typicalProductsResponse = typicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId
            }).ToList();
            return Ok(typicalProductsResponse);
        }

        [HttpGet("{id}")]

        public IActionResult GetProductsById(int id)
        {
            var typicalProduct = _context.TypicalProducts.FirstOrDefault(t => t.Id == id);

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

        [HttpPost]

        public IActionResult CreateTypicalProduct([FromBody] TypicalProductDto request)
        {
            if (request == null)
            {
                return BadRequest("The TypicalProduct cannot be null");
            }

            var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId);
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
            _context.TypicalProducts.Add(typicalProduct);
            _context.SaveChanges();
            return Ok(new {id = typicalProduct.Id});

        }

        [HttpPut("{id}")]
        public IActionResult UpdateTypicalProduct(int id, [FromBody] TypicalProductDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Name == null)
            {
                return BadRequest("El nombre del producto típico es nulo");
            }

            var existingTypicalProduct = _context.TypicalProducts.FirstOrDefault(tp => tp.Id == request.Id);
            if (existingTypicalProduct == null)
            {
                return NotFound($"El producto típico con ID {request.Id} no fue encontrado");
            }

            // Validar que la provincia exista (llave foránea)
            var provinceExists = _context.Provinces.Any(p => p.Id == request.ProvinceId);
            if (!provinceExists)
            {
                return BadRequest($"La provincia con ID {request.ProvinceId} no existe");
            }

            // Actualizar las propiedades
            existingTypicalProduct.Name = request.Name;
            existingTypicalProduct.Description = request.Description;
            existingTypicalProduct.ImageUrl = request.ImageUrl;
            existingTypicalProduct.ProvinceId = request.ProvinceId; // Actualizar la llave foránea

            _context.TypicalProducts.Update(existingTypicalProduct);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteTypicalProduct(int id)
        {
            var typicalProduct = _context.TypicalProducts.FirstOrDefault(t => t.Id == id);
            if (typicalProduct == null)
            {
                return NotFound($"TypicalProduct con ID: {id} no fue encontrada");
            }

            _context.TypicalProducts.Remove(typicalProduct);
            _context.SaveChanges();
            return NoContent();


        }
    }
}
