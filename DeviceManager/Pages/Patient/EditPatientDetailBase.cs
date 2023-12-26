using DeviceManager.Pages.Patient;
using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Pages.Patient

{
	public class EditPatientDetailBase : ComponentBase
	{
		[Inject] ISnackbar Snackbar { get; set; }
		protected MudForm form;
		protected PatientEntity model = new PatientEntity();
		protected PatientEntitylFluentValidator patientValidator = new PatientEntitylFluentValidator();
		protected IEnumerable<GenderEnum> genderTypes = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToList();
		protected IList<ProviderEntity> providers = new List<ProviderEntity>();

		[Inject] IPatientRepository patientRepository { get; set; }
		[Inject] IProviderRepository providerRepository { get; set; }
		[Inject]
		NavigationManager navigationManager { get; set; }
		[Inject] public StateDetails? stateDetails { get; set; }

		[Parameter]
        public string? Id { get; set; }

		

        protected override async Task OnInitializedAsync()
		{
			
			

			if(Guid.TryParse(Id, out Guid patientId))
			{
				providers = await providerRepository.GetAllProvider();
				var patientToBeEdited = await patientRepository.GetPatientById(patientId);
				if (patientToBeEdited != null)
				{
					model = patientToBeEdited;
				}
			}
			await base.OnInitializedAsync();
		}
		protected async Task Submit()
		{
			await form.Validate();
			if (form.IsValid)
			{
				Snackbar.Add("Updated!");
				
				if (await patientRepository.UpdatePatient(model))
				{
					navigationManager.NavigateTo("/patient");

				}

			}
		}

        protected void Cancel()
        {
            navigationManager.NavigateTo("/patient");
        }
    }
}
