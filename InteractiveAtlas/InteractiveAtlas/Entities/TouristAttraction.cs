using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("TouristAttractions")]
    public class TouristAttraction
    {
        [Column("AttractionId")]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? ImageUrl { get; set; }

        //Navigation

        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;


    }
}
