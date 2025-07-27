
using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Services
{
    public class QuizAnswerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizAnswerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QuizAnswerDto>> GetQuizAnswers()
        {
            var quizAnswers = await _unitOfWork.QuizAnswers.GetAllAsync();

            return quizAnswers.Select(a => new QuizAnswerDto
            {
                Id = a.Id,
                Text = a.Text,
                IsCorrect = a.IsCorrect,
                QuestionId = a.QuestionId
            }).ToList();
        }

        public async Task<List<QuizAnswerDto>> GetQuizAnswersWithQuestion()
        {
            var quizAnswers = await _unitOfWork.QuizAnswers.GetAllQuizAnswerWithQuestionAsync();

            var quizAnswersResponse = new List<QuizAnswerDto>();
            quizAnswersResponse = quizAnswers.Select(a => new QuizAnswerDto
            {
                Id = a.Id,
                Text = a.Text,
                IsCorrect = a.IsCorrect,
                QuestionId = a.QuestionId
            }).ToList();
            return quizAnswersResponse;
        }

        public async Task<QuizAnswerDto> GetQuizAnswerById(int id)
        {
            var quizAnswer = await _unitOfWork.QuizAnswers.GetQuizAnswerWithQuizQuestionByIdAsync(id);

            if (quizAnswer == null)
            {
                throw new Exception($"Quiz Answer with ID: {id} not found");
            }

            var quizAnswerResponse = new QuizAnswerDto
            {
                Id = quizAnswer.Id,
                Text = quizAnswer.Text,
                IsCorrect = quizAnswer.IsCorrect,
                QuestionId = quizAnswer.QuestionId
            };
            return quizAnswerResponse;
        }

        public async Task<List<QuizAnswerDto>> GetQuizAnswersByQuestionId(int questionId)
        {
            var quizAnswers = await _unitOfWork.QuizAnswers.GetAllQuizAnswerByQuestionIdAsync(questionId);

            return quizAnswers.Select(a => new QuizAnswerDto
            {
                Id = a.Id,
                Text = a.Text,
                IsCorrect = a.IsCorrect,
                QuestionId = a.QuestionId
            }).ToList();
        }

        public async Task<int> CreateQuizAnswer(QuizAnswerDto request)
        {
            if (request == null)
            {
                throw new Exception("The QuizAnswer cannot be null");
            }

            var questionExists = _unitOfWork.Context.QuizQuestions.Any(q => q.Id == request.QuestionId);
            if (!questionExists)
            {
                throw new Exception($"La pregunta con ID {request.QuestionId} no existe");
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new Exception("El texto de la respuesta es requerido");
            }

            var quizAnswer = new QuizAnswer
            {
                Text = request.Text,
                IsCorrect = request.IsCorrect,
                QuestionId = request.QuestionId,
            };

            quizAnswer = await _unitOfWork.QuizAnswers.AddAsync(quizAnswer);
            await _unitOfWork.CompleteAsync();

            return quizAnswer.Id;
        }

        public async Task UpdateQuizAnswer(int id, QuizAnswerDto request)
        {
            if (id != request.Id)
            {
                throw new Exception("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                throw new Exception("El texto de la respuesta es nulo");
            }

            var existingQuizAnswer = await _unitOfWork.QuizAnswers.GetQuizAnswerWithQuizQuestionByIdAsync(id);
            if (existingQuizAnswer == null)
            {
                throw new Exception($"La respuesta con ID {request.Id} no fue encontrada");
            }

            var questionExists = _unitOfWork.Context.QuizQuestions.Any(q => q.Id == request.QuestionId);
            if (!questionExists)
            {
                throw new Exception($"La pregunta con ID {request.QuestionId} no existe");
            }

            existingQuizAnswer.Text = request.Text;
            existingQuizAnswer.IsCorrect = request.IsCorrect;
            existingQuizAnswer.QuestionId = request.QuestionId;

            _unitOfWork.QuizAnswers.UpdateAsync(existingQuizAnswer).Wait();
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteQuizAnswer(int id)
        {
            var quizAnswer = await _unitOfWork.QuizAnswers.GetByIdAsync(id);
            if (quizAnswer == null)
            {
                throw new Exception($"QuizAnswer con ID: {id} no fue encontrada");
            }

            await _unitOfWork.QuizAnswers.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
