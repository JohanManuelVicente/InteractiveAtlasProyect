using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Domain.Entities
{
    [Table("TouristAttractions")]
    public class TouristAttraction
    {
        [Column("AttractionId")]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        [StringLength(250)]
        public string? Location { get; set; }
        [StringLength(400)]
        public string? ImageUrl { get; set; }

        //Navigation

        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;


    }
}
