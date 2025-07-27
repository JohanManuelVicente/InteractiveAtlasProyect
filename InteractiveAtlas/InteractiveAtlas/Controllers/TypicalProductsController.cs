using InteractiveAtlas.Application.Contracts;
using InteractiveAtlas.Application.DTOs;

using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Infrastucture.Data;
using InteractiveAtlas.Infrastucture.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TypicalProductsController : ControllerBase
    {
        private readonly ITypicalProductService _typicalProductService;

        public TypicalProductsController(ITypicalProductService typicalProductService)
        {
            _typicalProductService = typicalProductService;
        }
        [HttpGet]

        public async Task<IActionResult> GetTypicalProducts()
        {

            return Ok(await _typicalProductService.GetTypicalProducts());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetTypicalProductsWithProvince()
        {

            return Ok(await _typicalProductService.GetTypicalProductsWithProvince());
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetTypicalProductsById(int id)
        {
           
            return Ok(await _typicalProductService.GetTypicalProductsById(id));

        }

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GettypicalProductsByProvinceId([FromQuery]int provinceId)
        {
            return Ok(await _typicalProductService.GettypicalProductsByProvinceId(provinceId));

        }

           [HttpPost]

        public async Task<IActionResult> CreateTypicalProduct([FromBody] TypicalProductDto request)
        {
            var responseId = await _typicalProductService.CreateTypicalProduct(request);
            return Ok(new { id = responseId});

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypicalProduct(int id, [FromBody] TypicalProductDto request)
        {
            await _typicalProductService.UpdateTypicalProduct(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteTypicalProduct(int id)
        {
            await _typicalProductService.DeleteTypicalProduct(id);
            return NoContent();


        }
    }
}
