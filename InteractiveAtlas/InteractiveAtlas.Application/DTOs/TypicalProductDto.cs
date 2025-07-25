using System.ComponentModel.DataAnnotations.Schema;

namespace InteractiveAtlas.Application.DTOs
{
    public class TypicalProductDto 
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }


        public int ProvinceId { get; set; }
       
        public string? ProvinceName { get;  set; }
    }
}
