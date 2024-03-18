using Entities;

namespace DeviceManager.Repository
{
	public interface IBillingDetailsRepository
	{
		Task<bool> AddBillingDetails(BillingDetailsEntity billingDetails, BillingEntity billingEntity);
		Task<bool> UpdateBilling(BillingDetailsEntity billingDetails, BillingEntity billingEntity);
	}
}