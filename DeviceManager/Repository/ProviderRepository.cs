using Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly PatientManagementDbContext _patientManagementDbContext;
        public ProviderRepository(PatientManagementDbContext patientManagementDbContext)
        {
            _patientManagementDbContext = patientManagementDbContext;
        }

        public async Task<bool> AddProvider(ProviderEntity providerEntity)
        {
            if (!string.IsNullOrEmpty(providerEntity.PhoneNumber))
            {
                providerEntity.PhoneNumber = providerEntity.PhoneNumber.Replace(" ", "");
            }
            
            providerEntity.ProviderId = Guid.NewGuid();
            providerEntity.CreateDate = DateTime.Now;
            await _patientManagementDbContext.ProviderEntities.AddAsync(providerEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProvider(ProviderEntity providerEntity)
        {
            _patientManagementDbContext.ProviderEntities.Update(providerEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProvider(Guid id)
        {
            var provider = await _patientManagementDbContext.ProviderEntities.FindAsync(id);
            if (provider != null)
            {
				var claims = _patientManagementDbContext.ClaimEntities.Where(x => x.Provider == provider).ToList();
				if (claims != null)
				{
					_patientManagementDbContext.ClaimEntities.RemoveRange(claims);

				}

				_patientManagementDbContext.ProviderEntities.Remove(provider);
                await _patientManagementDbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ProviderEntity?> GetProviderById(Guid id)
        {
            return await _patientManagementDbContext.ProviderEntities.FindAsync(id);

        }

        public async Task<List<ProviderEntity>?> GetProviderByNameAndPhoneNumber(string name, string phoneNumber)
        {
           //var a =  await _patientManagementDbContext.ProviderEntities.FindAsync(name, phoneNumber);  
          var result=  _patientManagementDbContext.ProviderEntities.Where(x => x.LegalName.Equals(name) && x.PhoneNumber.Equals(phoneNumber));
            if(result != null)
            { 
               return await result.ToListAsync();
            }
            return null;
        }
        public List<ProviderEntity> GetAllProviderByNameOrNPI(string NameOrNPI)
        {
            List<ProviderEntity> nameProviderEntities = new List<ProviderEntity>();
            if (long.TryParse(NameOrNPI, out long npi))
            {
                nameProviderEntities = _patientManagementDbContext.ProviderEntities.Where(x => x.LegalName.Contains(NameOrNPI, StringComparison.OrdinalIgnoreCase) || x.NPI.ToString().Contains(NameOrNPI, StringComparison.OrdinalIgnoreCase)).ToList();

            }
            else
            {
                _patientManagementDbContext.ProviderEntities.Where(x => x.LegalName.Contains(NameOrNPI, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return nameProviderEntities;
        }

        public async Task<List<ProviderEntity>> GetAllProvider()
        {
           return await  _patientManagementDbContext.ProviderEntities.OrderByDescending(x => x.CreateDate).ToListAsync();
        }
    }
}
