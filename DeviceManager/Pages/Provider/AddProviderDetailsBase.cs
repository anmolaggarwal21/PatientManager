using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Pages.Provider
{
    public class AddProviderDetailsBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected ProviderEntity model = new ProviderEntity();
        protected ProviderEntitylFluentValidator providerValidator = new ProviderEntitylFluentValidator();

        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IProviderRepository providerRepository { get; set; }
		[Inject] public StateDetails? stateDetails { get; set; }
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                Snackbar.Add("Submited!");
                if(await providerRepository.AddProvider(model))
                {
                    navigationManager.NavigateTo("/provider");

				}

            }
        }

        protected void Cancel()
        {
            navigationManager.NavigateTo("/provider");
        }
    }
}
