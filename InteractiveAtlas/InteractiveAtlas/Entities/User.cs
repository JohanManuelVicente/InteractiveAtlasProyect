using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("Users")]
    public class User
    {
        [Column("UserId")]
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string? email { get; set; }

        public string Password { get; set; } = null!;

        public bool IsAdmin { get; set; }  = false;

        //Navigation
        public ICollection<User> Users { get; set; } = new List<User>();    

    }
}
