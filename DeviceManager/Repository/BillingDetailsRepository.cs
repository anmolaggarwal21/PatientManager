using Entities;

namespace DeviceManager.Repository
{
	public class BillingDetailsRepository : IBillingDetailsRepository
	{

		private readonly PatientManagementDbContext _patientManagementDbContext;
		public BillingDetailsRepository(PatientManagementDbContext patientManagementDbContext)
		{
			_patientManagementDbContext = patientManagementDbContext;
		}
		public async Task<bool> AddBillingDetails(BillingDetailsEntity billingDetails, BillingEntity billingEntity)
		{
			billingDetails.BillingDetailsId = Guid.NewGuid();
			billingEntity.BillingDetails = billingDetails;
			await _patientManagementDbContext.BillingDetailsEntities.AddAsync(billingDetails);
			_patientManagementDbContext.BillingEntities.Update(billingEntity);
			await _patientManagementDbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateBilling(BillingDetailsEntity billingDetails, BillingEntity billingEntity)
		{
			billingEntity.BillingDetails = billingDetails;
			_patientManagementDbContext.BillingDetailsEntities.Update(billingDetails);
			_patientManagementDbContext.BillingEntities.Update(billingEntity);
			await _patientManagementDbContext.SaveChangesAsync();
			return true;
		}

	}
}
