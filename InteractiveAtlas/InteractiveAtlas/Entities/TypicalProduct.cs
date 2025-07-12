using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("TypicalProducts")]
    public class TypicalProduct
    {
        [Column("ProductId")]
        public int Id { get; set; }

        public string Name { get; set; } = null!; 

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        //Navigation

        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; } = null!;


    }
}
