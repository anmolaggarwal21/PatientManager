using Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository
{
	public class ClaimRepository : IClaimRepository
	{
		private readonly PatientManagementDbContext _patientManagementDbContext;
		public ClaimRepository(PatientManagementDbContext patientManagementDbContext)
		{
			_patientManagementDbContext = patientManagementDbContext;
		}

		public async Task<bool> AddClaim(ClaimEntity claimEntity)
		{
			claimEntity.ClaimId = Guid.NewGuid();
			claimEntity.CreateDate = DateTime.Now;
			await _patientManagementDbContext.ClaimEntities.AddAsync(claimEntity);
			await _patientManagementDbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> UpdateClaim(ClaimEntity claimEntity)
		{
			_patientManagementDbContext.ClaimEntities.Update(claimEntity);
			await _patientManagementDbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteClaim(Guid id)
		{
			var claim = await _patientManagementDbContext.ClaimEntities.FindAsync(id);
			if (claim != null)
			{
				_patientManagementDbContext.ClaimEntities.Remove(claim);
				await _patientManagementDbContext.SaveChangesAsync();
				return true;
			}
			else
			{
				return false;
			}
		}
		public async Task<ClaimEntity?> GetClaimById(Guid id)
		{
			return await _patientManagementDbContext.ClaimEntities.Include(y => y.Patient).Where(x=> x.ClaimId == id).FirstOrDefaultAsync();

		}

		public async Task<List<ClaimEntity>>  GetClaimByPatientId(Guid id)
		{
            return await _patientManagementDbContext.ClaimEntities.Include(y => y.Patient).Where(x => x.Patient.PatientId == id).ToListAsync();
        }

		public async Task<List<ClaimEntity>> GetAllClaim()
		{
			return await _patientManagementDbContext.ClaimEntities.Include(y => y.Patient).OrderByDescending(x => x.CreateDate).ToListAsync();
		}
	}
}
