using InteractiveAtlas.Domain.Entities;

namespace InteractiveAtlas.Infrastucture.Contracts
{
    public interface IProvinceRepository : IRepository<Province>
    {
        Task<List<Province>> GetAllProvincesWithDetailsAsync();
    }
}