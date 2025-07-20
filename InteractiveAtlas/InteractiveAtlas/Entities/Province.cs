using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("Provinces")]
    public class Province
    {
        [Column("ProvinceId")]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;
        [StringLength(50)]
        public string Capital { get; set; }=null!;

        public float AreaKm2 { get; set; } // If put FLOAT, in DB should put like REAL type

        public int Population { get; set; }

        public float Density { get; set; }
        [StringLength(50)]
        public string Region { get; set; } = null!;

        public float  Latitude { get; set; }

        public float Longitude { get; set; }
        [StringLength(400)]
        public string? ImageUrl { get; set; }

        public string? Description { get; set; }


        // Navigation
        public virtual ICollection<TouristAttraction> TouristAttractions { get; set; } = new List<TouristAttraction>();

        public virtual ICollection<TypicalProduct> TypicalProducts { get; set; } = new List<TypicalProduct>();

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>(); 


    }
}
