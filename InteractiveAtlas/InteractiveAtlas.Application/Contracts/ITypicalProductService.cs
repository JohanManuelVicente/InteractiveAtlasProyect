using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Application.Contracts
{
    public interface ITypicalProductService
    {
        Task<int> CreateTypicalProduct(TypicalProductDto request);
        Task DeleteTypicalProduct(int id);
        Task<List<TypicalProductDto>> GetTypicalProducts();
        Task<TypicalProductDto> GetTypicalProductsById(int id);
        Task<List<TypicalProductDto>> GettypicalProductsByProvinceId(int provinceId);
        Task<List<TypicalProductDto>> GetTypicalProductsWithProvince();
        Task UpdateTypicalProduct(int id, TypicalProductDto request);
    }
}