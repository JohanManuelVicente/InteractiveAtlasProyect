using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Entities;
using System.Xml.Linq;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypicalProductsController : ControllerBase
    {
        private List<TypicalProduct> _typicalProducts;

        public TypicalProductsController()
        {
            _typicalProducts = new List<TypicalProduct>();
            _typicalProducts.Add(new TypicalProduct
            {
                Id = 1,
                ProvinceId = 1,
                Name = "Moro de guandules",
                Description = "Arroz con guandules típico de la zona sur.",
                ImageUrl = "/images/products/moro-guandules.jpg"
            });

            _typicalProducts.Add(new TypicalProduct
            {
                Id = 2,
                ProvinceId = 2,
                Name = "Casabe",
                Description = "Tortilla crujiente de yuca, tradicional en el Cibao.",
                ImageUrl = "/images/products/casabe.jpg"
            });


        }

        [HttpGet]

        public IActionResult GetTypicalProducts()
        {
            return Ok(_typicalProducts);
        }

        [HttpGet("{id}")]

        public IActionResult GetProductsById(int id)
        {
            var typicalProduct = _typicalProducts.FirstOrDefault(t => t.Id == id);

            if (typicalProduct == null)
            { return BadRequest($"Typical Product with id {id} not found"); }

            return Ok(typicalProduct);

        }


        //LLAVE FORANEA IMPORTANTE
    //    var dept = _context.Administration.FirstOrDefault(d => d.Id == employee.AdministrationId);
    //        if (dept == null)
    //        {
    //            return BadRequest("Departamento inválido");
    //}
}
}
