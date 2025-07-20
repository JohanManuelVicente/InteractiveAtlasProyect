using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("QuizAnswers")]
    public class QuizAnswer
    {
        [Column("AnswerId")]
        public int Id { get; set; }
        [StringLength(500)]
        public string Text { get; set; } = null!;

        public bool IsCorrect { get; set; }

        //Navigation

        public int QuestionId { get; set; }
        public virtual QuizQuestion Question { get; set; } = null!;

    }
}
