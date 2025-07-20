using System.ComponentModel.DataAnnotations.Schema;

namespace InteractiveAtlas.DTOs
{
    public class QuizQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? DifficultyLevel { get; set; }
        public int? ProvinceId { get; set; }

        public string? ProvinceName { get; set; }
    }
}
