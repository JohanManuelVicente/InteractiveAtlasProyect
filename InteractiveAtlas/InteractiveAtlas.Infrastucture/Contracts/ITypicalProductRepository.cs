using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface ITypicalProductRepository : IRepository<TypicalProduct>
    {
        Task<List<TypicalProduct>> GetAllTypicalProductByProvinceIdAsync(int provinceId);
        Task<List<TypicalProduct>> GetAllTypicalProductWithProvinceAsync();
        Task<TypicalProduct?> GetTypicalProductWithProvinceByIdAsync(int id);
    }
}