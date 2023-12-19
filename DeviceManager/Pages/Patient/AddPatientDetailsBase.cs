using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO;

namespace DeviceManager.Pages.Patient
{
    public class AddPatientDetailsBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected PatientEntity model = new PatientEntity();
        protected PatientEntitylFluentValidator patientValidator = new PatientEntitylFluentValidator();
		protected IEnumerable<GenderEnum> genderTypes = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToList();
		protected IList<ProviderEntity> providers = new List<ProviderEntity>();
        
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IPatientRepository patientRepository { get; set; }
		[Inject] IProviderRepository providerRepository { get; set; }
		[Inject] public StateDetails? stateDetails { get; set; }
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

				if (await patientRepository.AddPatient(model))
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
