using Entities;

namespace DeviceManager.Repository
{
    public interface IBillingRepository
    {
        Task<bool> AddBilling(BillingEntity billingEntity);
        Task<List<BillingEntity>> GetAllBilling();
    }
}