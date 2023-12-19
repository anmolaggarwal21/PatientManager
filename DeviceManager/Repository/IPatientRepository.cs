using Entities;

namespace DeviceManager.Repository
{
    public interface IPatientRepository
    {
        Task<bool> AddPatient(PatientEntity patientEntity);
        Task<bool> DeletePatient(Guid id);
        Task<List<PatientEntity>> GetAllPatient();
        Task<PatientEntity?> GetPatientById(Guid id);
        Task<bool> UpdatePatient(PatientEntity patientEntity);

        Task<List<PatientEntity>> GetPatientByProviderId(Guid id);
    }
}