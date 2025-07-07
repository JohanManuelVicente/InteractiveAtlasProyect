using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InteractiveAtlas.Entities
{
    [Table("QuizResults")]
    public class QuizResult
    {
        [Column("ResultId")]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Score { get; set; }

        public int TotalQuestions { get; set; }

        public DateTime DateTaken { get; set; } = DateTime.Now;

        
    }
}
