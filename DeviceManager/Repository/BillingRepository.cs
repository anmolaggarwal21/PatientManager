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
            billingEntity.CreateDate = DateTime.Now;
            await _patientManagementDbContext.BillingEntities.AddAsync(billingEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBilling(BillingEntity billingEntity)
        {
            _patientManagementDbContext.BillingEntities.Update(billingEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<BillingEntity>> GetAllBilling()
        {
            return await _patientManagementDbContext.BillingEntities.Include(y => y.Patient).Include(z => z.BillingDetails).OrderByDescending(x => x.CreateDate).ToListAsync();
        }

        public async Task<BillingEntity?> GetBillingById(Guid id)
        {
            return await _patientManagementDbContext.BillingEntities.Include(y => y.Patient).Where(x => x.BillingId == id).FirstOrDefaultAsync();

        }

        public async Task<bool> DeleteBilling(Guid id)
        {
            var billing = await _patientManagementDbContext.BillingEntities.FindAsync(id);
            if (billing != null)
            {
                _patientManagementDbContext.BillingEntities.Remove(billing);
                await _patientManagementDbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
