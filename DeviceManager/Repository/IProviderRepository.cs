using Entities;

namespace DeviceManager.Repository
{
    public interface IProviderRepository
    {
        Task<bool> AddProvider(ProviderEntity providerEntity);
        Task<bool> DeleteProvider(Guid id);
        List<ProviderEntity> GetAllProviderByNameOrNPI(string NameOrNPI);
        Task<ProviderEntity?> GetProviderById(Guid id);
        Task<bool> UpdateProvider(ProviderEntity providerEntity);
        Task<List<ProviderEntity>> GetAllProvider();
        Task<List<ProviderEntity>?> GetProviderByNameAndPhoneNumber(string name, string phoneNumber);
    }
}