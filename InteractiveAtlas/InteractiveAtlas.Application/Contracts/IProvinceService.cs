using InteractiveAtlas.Application.DTOs;

namespace InteractiveAtlas.Application.Contracts
{
    public interface IProvinceService
    {
        Task<int> CreateProvince(ProvinceDto request);
        Task DeleteProvince(int id);
        Task<ProvinceDto> GetProvinceById(int id);
        Task<List<ProvinceDto>> GetProvinces();
        Task<List<ProvinceDto>> GetProvincesWithTypicalProducts();
        Task UpdateProvince(int id, ProvinceDto request);
    }
}