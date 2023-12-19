using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO;

namespace DeviceManager.Pages.Claim
{
    public class EditClaimDetailsBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected ClaimEntity model = new ClaimEntity();
        protected ClaimEntitylFluentValidator claimValidator = new ClaimEntitylFluentValidator();
		protected IList<ProviderEntity> providers = new List<ProviderEntity>();
        protected IList<PatientEntity> patients = new List<PatientEntity>();

        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IPatientRepository patientRepository { get; set; }
		[Inject] IProviderRepository providerRepository { get; set; }
        [Inject] IClaimRepository claimRepository { get; set; }
		[Parameter]
		public string? Id { get; set; }

		protected override async Task OnInitializedAsync()
		{
			if (Guid.TryParse(Id, out Guid claimId))
			{
				var claimIdToBeEdited = await claimRepository.GetClaimById(claimId);
				if (claimIdToBeEdited != null)
				{
					model = claimIdToBeEdited;
                    patients = await patientRepository.GetPatientByProviderId(claimIdToBeEdited.Provider.ProviderId);
                }
			}

			providers =  await providerRepository.GetAllProvider();
            
			await base.OnInitializedAsync();
		}
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                Snackbar.Add("Updated!");

				if (await claimRepository.UpdateClaim(model))
                {
                    navigationManager.NavigateTo("/claim");

				}

            }
        }

		protected async Task onValueChanged(ProviderEntity selected)
		{
            model.Patient = null;
            model.Provider = selected;
			patients = await patientRepository.GetPatientByProviderId(selected.ProviderId);
			// Do other stuff
		}
		protected void Cancel()
        {
            navigationManager.NavigateTo("/claim");
        }
    }
}
