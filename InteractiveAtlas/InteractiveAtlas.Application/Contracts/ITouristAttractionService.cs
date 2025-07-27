using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Application.Contracts
{
    public interface ITouristAttractionService
    {
        Task<int> CreateTouristAttraction(TouristAttractionDto request);
        Task DeleteTouristAttraction(int id);
        Task<TouristAttractionDto> GetTouristAttractionById(int id);
        Task<List<TouristAttractionDto>> GetTouristAttractions();
        Task<List<TouristAttractionDto>> GetTouristAttractionsByProvinceId(int provinceId);
        Task<List<TouristAttractionDto>> GetTouristAttractionsWithProvince();
        Task UpdateTouristAttraction(int id, TouristAttractionDto request);
    }
}