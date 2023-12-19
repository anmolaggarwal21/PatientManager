using DeviceManager.Enums;
using DeviceManager.Repository;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Shared
{
	public class DeleteDialogBase : ComponentBase
	{
		[CascadingParameter] MudDialogInstance MudDialog { get; set; }

		[Parameter] public string ContentText { get; set; }

		[Parameter] public string ButtonText { get; set; }

		[Parameter] public Color Color { get; set; }
		[Parameter] public EntityTypeEnum entityTypeEnum { get; set; }

        [Parameter] public string Id { get; set; }

        
        [Inject] IProviderRepository providerRepository { get; set; }
		[Inject] IPatientRepository patientRepository { get; set; }
		[Inject] IClaimRepository claimRepository { get; set; }
		[Inject]
        NavigationManager navigationManager { get; set; }
        protected async Task Submit()
		{

			switch (entityTypeEnum)
			{
				case EntityTypeEnum.Provider:

					if(Guid.TryParse(Id,out Guid providerId))
					{
                       await providerRepository.DeleteProvider(providerId);
						navigationManager.NavigateTo("/provider", true);
                    }
					break;
				case EntityTypeEnum.Patient:
					if (Guid.TryParse(Id, out Guid patientId))
					{
						await patientRepository.DeletePatient(patientId);
						navigationManager.NavigateTo("/patient", true);
					}
					
					break;
				case EntityTypeEnum.Claim:
					if (Guid.TryParse(Id, out Guid claimId))
					{
						await claimRepository.DeleteClaim(claimId);
						navigationManager.NavigateTo("/claim", true);
					}

					break;

			}

            MudDialog.Close(DialogResult.Ok(true));
		} 
		protected void Cancel() => MudDialog.Cancel();
	}
}
