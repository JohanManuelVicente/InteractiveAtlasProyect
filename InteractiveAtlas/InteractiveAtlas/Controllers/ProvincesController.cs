using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Domain.Entities;
using System.Xml.Linq;
using InteractiveAtlas.DTOs;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using InteractiveAtlas.Infrastucture;
using InteractiveAtlas.Infrastucture.Repositories;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public ProvincesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _unitOfWork.Province.GetAllAsync();

            var provincesResponse = provinces.Select(p => new ProvinceDto
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

        [HttpGet]
        [Route("with-details")]
        public async Task<IActionResult> GetProvincesWithTypicalProducts()
        {
            var provinces = await _unitOfWork.Province.GetAllProvincesWithDetailsAsync();

            var provincesResponse = provinces.Select(p => new ProvinceDto
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
                Description = p.Description,
                TypicalProducts = p.TypicalProducts?.Select(tp => new TypicalProductDto
                {
                    Id = tp.Id,
                    Name = tp.Name,
                    Description = tp.Description,
                    ImageUrl = tp.ImageUrl,
                    ProvinceId = tp.ProvinceId
                }).ToList(),
                TouristAttractions = p.TouristAttractions?.Select(ta => new TouristAttractionDto
                {
                    Id = ta.Id,
                    Name = ta.Name,
                    Description = ta.Description,
                    ImageUrl = ta.ImageUrl,
                    ProvinceId = ta.ProvinceId
                }).ToList()

            }).ToList();

            return Ok(provincesResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProvinceById(int id)
        {
            var province = await _unitOfWork.Province.GetByIdAsync(id);

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
        }

        [HttpPost]
        public async Task<IActionResult> CreateProvince([FromBody] ProvinceDto request)
        {
            if (request == null)
            {
                return BadRequest("The Province cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("El nombre de la provincia es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Capital))
            {
                return BadRequest("La capital de la provincia es requerida");
            }

            if (string.IsNullOrWhiteSpace(request.Region))
            {
                return BadRequest("La región de la provincia es requerida");
            }

            var province = new Province
            {
                Name = request.Name,
                Capital = request.Capital,
                AreaKm2 = request.AreaKm2,
                Population = request.Population,
                Density = request.Density,
                Region = request.Region,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ImageUrl = request.ImageUrl,
                Description = request.Description
            };

            province = await _unitOfWork.Province.AddAsync(province);
            await _unitOfWork.CompleteAsync();
            return Ok(new { id = province.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvince(int id, [FromBody] ProvinceDto request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la petición");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("El nombre de la provincia es requerido");
            }

            if (string.IsNullOrWhiteSpace(request.Capital))
            {
                return BadRequest("La capital de la provincia es requerida");
            }

            if (string.IsNullOrWhiteSpace(request.Region))
            {
                return BadRequest("La región de la provincia es requerida");
            }

            var existingProvince = await _unitOfWork.Province.GetByIdAsync(id);
            if (existingProvince == null)
            {
                return NotFound($"La provincia con ID {request.Id} no fue encontrada");
            }

            // Actualizar propiedades
            existingProvince.Name = request.Name;
            existingProvince.Capital = request.Capital;
            existingProvince.AreaKm2 = request.AreaKm2;
            existingProvince.Population = request.Population;
            existingProvince.Density = request.Density;
            existingProvince.Region = request.Region;
            existingProvince.Latitude = request.Latitude;
            existingProvince.Longitude = request.Longitude;
            existingProvince.ImageUrl = request.ImageUrl;
            existingProvince.Description = request.Description;

            await _unitOfWork.Province.UpdateAsync(existingProvince);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProvince(int id)
        {
            var province = await _unitOfWork.Province.GetByIdAsync(id);
            if (province == null)
            {
                return NotFound($"Provincia con ID: {id} no fue encontrada");
            }

            await _unitOfWork.Province.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
