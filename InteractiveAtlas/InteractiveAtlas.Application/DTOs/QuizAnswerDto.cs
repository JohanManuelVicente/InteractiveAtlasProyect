using System.ComponentModel.DataAnnotations.Schema;

namespace InteractiveAtlas.Application.DTOs
{
    public class QuizAnswerDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
