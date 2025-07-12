using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("QuizQuestions")]
    public class QuizQuestion
    {
        [Column("QuestionId")]
        public int Id { get; set; }

        public string Text { get; set; } = null!;
                
        public string? DifficultyLevel { get; set; }


        //Navigation
        public string? ProvinceId { get; set; }
        public virtual Province? Province { get; set; } = null!;
        public virtual ICollection<QuizAnswer> Answers { get; set; } = new HashSet<QuizAnswer>();



    }
}
