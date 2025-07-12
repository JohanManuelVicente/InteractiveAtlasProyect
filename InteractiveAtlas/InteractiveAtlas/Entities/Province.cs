using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("Provinces")]
    public class Province
    {
        [Column("ProvinceId")]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Capital { get; set; }=null!;

        public float AreaKm2 { get; set; }

        public int Population { get; set; }

        public float Density { get; set; }

        public string Region { get; set; } = null!;

        public float  Latitude { get; set; }

        public float Longitude { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }


        // Navigation
        public virtual ICollection<TouristAttraction> TouristAttractions { get; set; } = new List<TouristAttraction>();

        public virtual ICollection<TypicalProduct> TypicalProducts { get; set; } = new HashSet<TypicalProduct>();

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();


    }
}
