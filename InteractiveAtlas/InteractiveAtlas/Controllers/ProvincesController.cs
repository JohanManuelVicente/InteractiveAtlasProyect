using Microsoft.AspNetCore.Mvc;
using InteractiveAtlas.Entities;
using System.Xml.Linq;

namespace InteractiveAtlas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {

        private List<Province> _provinces;

        public ProvincesController()
        {
            _provinces = new List<Province>();
            _provinces.Add(new Province
            {
                Id = 1,
                Name = "Santo Domingo",
                Capital = "Santo Domingo Este",
                AreaKm2 = 1300.35f,
                Population = 3200000,
                Density = 2461.5f,
                Region = "Sur",
                Latitude = 18.4861f,
                Longitude = -69.9312f,
                Description = "Es la provincia más poblada y centro político, económico y cultural del país.",
                ImageUrl = "/images/provinces/santo-domingo.jpg"
            });

            _provinces.Add(new Province
            {
                Id = 2,
                Name = "Santiago",
                Capital = "Santiago de los Caballeros",
                AreaKm2 = 2806.34f,
                Population = 1020000,
                Density = 363.5f,
                Region = "Norte",
                Latitude = 19.4517f,
                Longitude = -70.6989f,
                Description = "Conocida por su producción agrícola, zonas industriales y su cultura vibrante.",
                ImageUrl = "/images/provinces/santiago.jpg"
            });




        }


        [HttpGet]

        public IActionResult GetProvinces()
        {
            return Ok(_provinces);
        }

        [HttpGet("{id}")]

        public IActionResult GetProvinceById(int id)
        {
            var province = _provinces.FirstOrDefault(province => province.Id == id);
            //province = _provinces.Where(province => province.Id == id).FirstOrDefault();

            if (province == null)
            {
                return BadRequest($"Province with {id} not found");
            }
            return Ok(province);

        }

        [HttpPost]

        public IActionResult CreateProvince([FromBody] Province province)
        {
            if (province == null)
            {
                return BadRequest("The Province cannot be null");
            }

            province.Id = _provinces.Count + 1;
            _provinces.Add(province);
            return Ok(province);

            /*province.Id = _provinces.Count +1;
            //province.Name = Name;
            //province.Capital = "Barahona";
            //province.AreaKm2 = 1200.35f;
            //province.Population = 5300000;
            //    province.Density = 2461.5f;
            //    province.Region = "Sur";
            //    province.Latitude = 19.4861f;
            //    province.Longitude = -67.9312f;
            //    province.Description = "Es uno de los pueblos más importantes de la parte sur de la isla, con un puerto de intensa actividad y una amplia oferta de atracciones ecoturísticas";
            //province.ImageUrl = "/images/provinces/barahona.jpg";
            */

        }

        [HttpPut("{id}")]

        public IActionResult UpdateProvince(int id, [FromBody] Province province)
        {
            if(province.Name == null)
            {
                return BadRequest("El nombre de la provincia es nulo o ID no coincide");
            }

            var existingProvince = _provinces.FirstOrDefault(prov => prov.Id == id);
            if (existingProvince == null)
            {
                return NotFound($" La provincia con ID {province.Id} no fue encontrado");
            }

            if (province.Capital == null)
            {
                return BadRequest("La capital de la provincia es nulo o ID no coincide");
            }

            if (province.Region == null)
            {
                return BadRequest("La region de la provincia es nulo o ID no coincide");
            }

            existingProvince.Name = province.Name;
            existingProvince.Capital = province.Capital;
            existingProvince.AreaKm2    = province.AreaKm2;
            existingProvince.Population = province.Population;
            existingProvince.Density = province.Density;
            existingProvince.Region = province.Region;
            existingProvince.Latitude = province.Latitude;
            existingProvince.Longitude = province.Longitude;
            existingProvince.ImageUrl = province.ImageUrl;
            existingProvince.Description = province.Description;
            _provinces.Add(existingProvince);
            return Ok(existingProvince);


        }

        [HttpDelete("{id}")]

        public IActionResult DeleteProvince (int id)
        {
            var province = _provinces.FirstOrDefault(p => p.Id == id);
            if (province == null)
            {
                return NotFound($"Provincia con ID: {id} no fue encontrada");
            }

                _provinces.Remove(province);
            return Ok(_provinces);


        }
    }
}
