using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO;

namespace DeviceManager.Pages.Claim
{
    public class AddClaimDetailsBase : ComponentBase
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
        
		protected override async Task OnInitializedAsync()
		{
			providers =  await providerRepository.GetAllProvider();

			await base.OnInitializedAsync();
		}
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                Snackbar.Add("Submited!");

				if (await claimRepository.AddClaim(model))
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
