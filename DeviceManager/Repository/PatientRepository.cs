using Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientManagementDbContext _patientManagementDbContext;
        public PatientRepository(PatientManagementDbContext patientManagementDbContext)
        {
            _patientManagementDbContext = patientManagementDbContext;
        }

        public async Task<bool> AddPatient(PatientEntity patientEntity)
        {
            if (!string.IsNullOrEmpty(patientEntity.PhoneNumber))
            {
                patientEntity.PhoneNumber = patientEntity.PhoneNumber.Replace(" ", "");
            }

            patientEntity.PatientId = Guid.NewGuid();
            patientEntity.CreateDate = DateTime.Now;
            await _patientManagementDbContext.PatientEntities.AddAsync(patientEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePatient(PatientEntity patientEntity)
        {
            _patientManagementDbContext.PatientEntities.Update(patientEntity);
            await _patientManagementDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePatient(Guid id)
        {
            var patient = await _patientManagementDbContext.PatientEntities.FindAsync(id);
            if (patient != null)
            {
                _patientManagementDbContext.PatientEntities.Remove(patient);
                await _patientManagementDbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<PatientEntity?> GetPatientById(Guid id)
        {
            return await _patientManagementDbContext.PatientEntities.FindAsync(id);

        }
        

        public async Task<List<PatientEntity>> GetPatientByProviderId(Guid id)
        {
            return await _patientManagementDbContext.PatientEntities.Where(x => x.Provider.ProviderId == id).OrderByDescending(x => x.CreateDate).ToListAsync();

		}

		public async Task<List<PatientEntity>> GetAllPatient()
        {
            return await _patientManagementDbContext.PatientEntities.Include(x=> x.Provider).OrderByDescending(x=> x.CreateDate).ToListAsync();
        }
    }

}
