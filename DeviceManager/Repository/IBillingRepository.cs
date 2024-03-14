using Entities;

namespace DeviceManager.Repository
{
    public interface IBillingRepository
    {
        Task<bool> AddBilling(BillingEntity billingEntity);
        Task<bool> DeleteBilling(Guid id);
        Task<List<BillingEntity>> GetAllBilling();
        Task<BillingEntity?> GetBillingById(Guid id);
        Task<bool> UpdateBilling(BillingEntity claimEntity);
    }
}