using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Domain.Entities
{
    [Table("QuizQuestions")]
    public class QuizQuestion
    {
        [Column("QuestionId")]
        public int Id { get; set; }

        public string Text { get; set; } = null!;
        [StringLength(50)]
        public string? DifficultyLevel { get; set; }


        //Navigation
        public int? ProvinceId { get; set; }
        public virtual Province? Province { get; set; } = null!;
        public virtual ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();



    }
}
