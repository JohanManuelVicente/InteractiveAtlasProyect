using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("QuizQuestions")]
    public class QuizQuestion
    {
        [Column("QuestionId")]
        public int Id { get; set; }

        public string text { get; set; } = null!;

        public string? ProvinceId { get; set; }

        public string? DifficultyLevel { get; set; }


    }
}
