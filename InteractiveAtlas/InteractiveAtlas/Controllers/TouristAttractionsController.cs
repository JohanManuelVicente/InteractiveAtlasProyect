using InteractiveAtlas.Application.Contracts;
using InteractiveAtlas.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TouristAttractionsController : ControllerBase
    {
        private readonly ITouristAttractionService _touristAttractionService;

        public TouristAttractionsController(ITouristAttractionService touristAttractionService)
        {
            _touristAttractionService = touristAttractionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTouristAttractions()
        {
            return Ok(await _touristAttractionService.GetTouristAttractions());
        }

        [HttpGet]
        [Route("with-province")]
        public async Task<IActionResult> GetTouristAttractionsWithProvince()
        {

            return Ok(await _touristAttractionService.GetTouristAttractionsWithProvince());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTouristAttractionById(int id)
        {
            return Ok(await _touristAttractionService.GetTouristAttractionById(id));
        }

        [HttpGet]
        [Route("by-province")]
        public async Task<IActionResult> GetTouristAttractionsByProvinceId([FromQuery] int provinceId)
        {
            return Ok(await _touristAttractionService.GetTouristAttractionsByProvinceId(provinceId));
        }

        [HttpPost]

        public async Task<IActionResult> CreateTouristAttraction([FromBody] TouristAttractionDto request)
        {
            var responseId = await _touristAttractionService.CreateTouristAttraction(request);
            return Ok(new { id = responseId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTouristAttraction(int id, [FromBody] TouristAttractionDto request)
        {
            await _touristAttractionService.UpdateTouristAttraction(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTouristAttraction(int id)
        {
            await _touristAttractionService.DeleteTouristAttraction(id);
            return NoContent();
        }
    }
}
