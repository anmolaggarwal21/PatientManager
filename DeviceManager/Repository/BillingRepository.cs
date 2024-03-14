using Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository
{
    public class BillingRepository : IBillingRepository
    {
        private readonly PatientManagementDbContext _patientManagementDbContext;
        public BillingRepository(PatientManagementDbContext patientManagementDbContext)
        {
            _patientManagementDbContext = patientManagementDbContext;
        }

        public async Task<bool> AddBilling(BillingEntity billingEntity)
        {
            billingEntity.BillingId = Guid.NewGuid();
            // billingEntity.CreatedDate = DateTime.Now;
            await _patientManagementDbContext.BillingEntities.AddAsync(billingEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<BillingEntity>> GetAllBilling()
        {
            try
            {
                return await _patientManagementDbContext.BillingEntities.ToListAsync();
            }
            catch(Exception ex)
            {

            }
           return  await _patientManagementDbContext.BillingEntities.ToListAsync();
        }

    }
}
