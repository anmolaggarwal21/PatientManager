using Entities;

namespace DeviceManager.Repository
{
	public interface IClaimRepository
	{
		Task<bool> AddClaim(ClaimEntity claimEntity);
		Task<bool> DeleteClaim(Guid id);
		Task<List<ClaimEntity>> GetAllClaim();
		Task<ClaimEntity?> GetClaimById(Guid id);
		Task<bool> UpdateClaim(ClaimEntity claimEntity);
		Task<List<ClaimEntity>> GetClaimByPatientId(Guid id);
	}
}