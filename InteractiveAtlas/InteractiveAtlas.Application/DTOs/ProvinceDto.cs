using System.ComponentModel.DataAnnotations.Schema;

namespace InteractiveAtlas.Application.DTOs
{
    public class ProvinceDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Capital { get; set; } = null!;

        public float AreaKm2 { get; set; } // If put FLOAT, in DB should put like REAL type

        public int Population { get; set; }

        public float Density { get; set; }

        public string Region { get; set; } = null!;

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }
        public List<TypicalProductDto>? TypicalProducts { get;  set; }
       
        public List<TouristAttractionDto>? TouristAttractions { get; set; }
    }
}
