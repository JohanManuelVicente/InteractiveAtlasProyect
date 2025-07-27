using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using InteractiveAtlas.Infrastucture;
using InteractiveAtlas.Infrastucture.Repositories;
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.Contracts;
using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
         
            return Ok( await _provinceService.GetProvinces());
        }

        [HttpGet]
        [Route("with-details")]
        public async Task<IActionResult> GetProvincesWithTypicalProducts()
        {
            return Ok(await _provinceService.GetProvincesWithTypicalProducts());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProvinceById(int id)
        {
            return Ok(await _provinceService.GetProvinceById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProvince([FromBody] ProvinceDto request)
        {
            var responseId = await _provinceService.CreateProvince(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvince(int id, [FromBody] ProvinceDto request)
        {
            await _provinceService.UpdateProvince(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
           await _provinceService.DeleteProvince(id);
            return NoContent();
        }
    }
}
