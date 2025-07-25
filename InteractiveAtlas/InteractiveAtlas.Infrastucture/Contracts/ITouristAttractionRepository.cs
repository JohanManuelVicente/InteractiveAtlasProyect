using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface ITouristAttractionRepository : IRepository<TouristAttraction>
    {
        Task<List<TouristAttraction>> GetAllTouristAttractionByProvinceIdAsync(int provinceId);
        Task<List<TouristAttraction>> GetAllTouristAttractionWithProvinceAsync();
        Task<TouristAttraction?> GetTouristAttractionWithProvinceByIdAsync(int id);
    }
}