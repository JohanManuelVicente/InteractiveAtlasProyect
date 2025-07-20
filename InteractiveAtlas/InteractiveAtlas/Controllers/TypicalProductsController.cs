using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.DTOs;
using InteractiveAtlas.Infrastucture;
using InteractiveAtlas.Infrastucture.Repository;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypicalProductsController : ControllerBase
    {
        private readonly InteractiveAtlasDbContext _context;
        private readonly ProvinceRepository _provinceRepository;
        private readonly TypicalProductsRepository _typicalProductsRepository;

        public TypicalProductsController(InteractiveAtlasDbContext context, ProvinceRepository provinceRepository, TypicalProductsRepository typicalProductsRepository /*Put others repositories*/ )
        {
            _context = context;
            _provinceRepository = provinceRepository;
            _typicalProductsRepository = typicalProductsRepository;

        }
        [HttpGet]

        public async Task<IActionResult> GetTypicalProducts()
        {

            return Ok(await _typicalProductsRepository.GetAllTypicalProductAsync());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetTypicalProductsWithProvince()
        {

            var typicalProducts = await _typicalProductsRepository.GetAllTypicalProductWithProvinceAsync();


            var typicalProductsResponse = new List<TypicalProductDto>();
            typicalProductsResponse = typicalProducts.Select(t => new TypicalProductDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ImageUrl = t.ImageUrl,
                ProvinceId = t.ProvinceId,
                ProvinceName = t.Province.Name
                //Province = new ProvinceDto
                //{
                //    Id = t.Province.Id,
                //    Name = t.Province.Name,
                //    Capital = t.Province.Capital
                //}

            }).ToList();
            return Ok(typicalProductsResponse);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetTypicalProductsById(int id)
        {
            var typicalProduct = await _typicalProductsRepository.GetTypicalProductByIdAsync(id);

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

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GettypicalProductsByProvinceId([FromQuery]int provinceId)
        {
            return Ok(await _typicalProductsRepository.GetAllTypicalProductByProvinceIdAsync(provinceId));

        }

        [HttpPost]

        public async Task<IActionResult> CreateTypicalProduct([FromBody] TypicalProductDto request)
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
            typicalProduct = await _typicalProductsRepository.AddTypicalProductAsync(typicalProduct);

            return Ok(new { id = typicalProduct.Id });

        }

        [HttpPut("{id}")]
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

            //var existingTypicalProduct = await _typicalProductsRepository.GetTypicalProductByIdAsync(id).Result; Way to put a Async method without make completely the method async
            var existingTypicalProduct = await _typicalProductsRepository.GetTypicalProductByIdAsync(id);
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

            //_typicalProductsRepository.UpdateTypicalProductAsync(existingTypicalProduct).Wait();
            _typicalProductsRepository.UpdateTypicalProductAsync(existingTypicalProduct).Wait();
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteTypicalProduct(int id)
        {
            var typicalProduct = await _typicalProductsRepository.GetTypicalProductByIdAsync(id);
            if (typicalProduct == null)
            {
                return NotFound($"TypicalProduct con ID: {id} no fue encontrada");
            }

            await _typicalProductsRepository.DeleteTypicalProductAsync(id);
            return NoContent();


        }
    }
}
