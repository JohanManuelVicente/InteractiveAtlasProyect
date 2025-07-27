using InteractiveAtlas.Infrastucture.Contracts;
using InteractiveAtlas.Application.DTOs;
using InteractiveAtlas.Domain.Entities;
using InteractiveAtlas.Application.Contracts;

namespace InteractiveAtlas.Services
{
    public class QuizQuestionService : IQuizQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizQuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QuizQuestionDto>> GetQuizQuestions()
        {
            var quizQuestions = await _unitOfWork.QuizQuestions.GetAllAsync();

            return quizQuestions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                DifficultyLevel = q.DifficultyLevel,
                ProvinceId = q.ProvinceId
            }).ToList();
        }

        public async Task<List<QuizQuestionDto>> GetQuizQuestionsWithProvince()
        {
            var quizQuestions = await _unitOfWork.QuizQuestions.GetAllQuizQuestionWithProvinceAsync();

            var quizQuestionsResponse = new List<QuizQuestionDto>();
            quizQuestionsResponse = quizQuestions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                DifficultyLevel = q.DifficultyLevel,
                ProvinceId = q.ProvinceId,
                ProvinceName = q.Province?.Name
            }).ToList();
            return quizQuestionsResponse;
        }

        public async Task<QuizQuestionDto> GetQuizQuestionById(int id)
        {
            var quizQuestion = await _unitOfWork.QuizQuestions.GetQuizQuestionWithProvinceByIdAsync(id);

            if (quizQuestion == null)
            {
                throw new Exception($"Quiz Question with ID: {id} not found");
            }

            var quizQuestionResponse = new QuizQuestionDto
            {
                Id = quizQuestion.Id,
                Text = quizQuestion.Text,
                DifficultyLevel = quizQuestion.DifficultyLevel,
                ProvinceId = quizQuestion.ProvinceId
            };
            return quizQuestionResponse;
        }

        public async Task<List<QuizQuestionDto>> GetQuizQuestionsByProvinceId(int provinceId)
        {
            var quizQuestions = await _unitOfWork.QuizQuestions.GetAllQuizQuestionByProvinceIdAsync(provinceId);

            return quizQuestions.Select(q => new QuizQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                DifficultyLevel = q.DifficultyLevel,
                ProvinceId = q.ProvinceId
            }).ToList();
        }

        public async Task<int> CreateQuizQuestion(QuizQuestionDto request)
        {
            if (request == null)
            {
                throw new Exception("The QuizQuestion cannot be null");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    throw new Exception($"La provincia con ID {request.ProvinceId} no existe");
                }
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                throw new Exception("El texto de la pregunta es requerido");
            }

            var quizQuestion = new QuizQuestion
            {
                Text = request.Text,
                DifficultyLevel = request.DifficultyLevel,
                ProvinceId = request.ProvinceId,
            };

            quizQuestion = await _unitOfWork.QuizQuestions.AddAsync(quizQuestion);
            await _unitOfWork.CompleteAsync();

            return quizQuestion.Id;
        }

        public async Task UpdateQuizQuestion(int id, QuizQuestionDto request)
        {
            if (id != request.Id)
            {
                throw new Exception("El ID de la URL no coincide con el ID de la peticion");
            }

            if (request.Text == null)
            {
                throw new Exception("El texto de la pregunta es nulo");
            }

            var existingQuizQuestion = await _unitOfWork.QuizQuestions.GetQuizQuestionWithProvinceByIdAsync(id);
            if (existingQuizQuestion == null)
            {
                throw new Exception($"La pregunta con ID {request.Id} no fue encontrada");
            }

            if (request.ProvinceId.HasValue)
            {
                var provinceExists = _unitOfWork.Context.Provinces.Any(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    throw new Exception($"La provincia con ID {request.ProvinceId} no existe");
                }
            }

            existingQuizQuestion.Text = request.Text;
            existingQuizQuestion.DifficultyLevel = request.DifficultyLevel;
            existingQuizQuestion.ProvinceId = request.ProvinceId;

            _unitOfWork.QuizQuestions.UpdateAsync(existingQuizQuestion).Wait();
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteQuizQuestion(int id)
        {
            var quizQuestion = await _unitOfWork.QuizQuestions.GetByIdAsync(id);
            if (quizQuestion == null)
            {
                throw new Exception($"QuizQuestion con ID: {id} no fue encontrada");
            }

            await _unitOfWork.QuizQuestions.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
        }
    }
}
