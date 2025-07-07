using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("Provinces")]
    public class Province
    {
        public int provinceId { get; set; }

        public string name { get; set; }

        public string Capital { get; set; }

        public float areaKm2 { get; set; }

        public int population { get; set; }

        public float density { get; set; }

        public string region { get; set; }

        public float  latitude { get; set; }

        public float longitude { get; set; }

        public string imageUrl { get; set; }

        public string description { get; set; }


    }
}
