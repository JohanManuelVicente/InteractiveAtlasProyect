using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("TouristAttractions")]
    public class TouristAttraction
    {
        public int Id { get; set; }

        public int ProvinceId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? ImageUrl { get; set; }


    }
}
