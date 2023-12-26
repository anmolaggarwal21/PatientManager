using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Pages.Provider
{
	public class EditProviderDetailBase : ComponentBase
	{
		[Inject] ISnackbar Snackbar { get; set; }
		protected MudForm form;
		protected ProviderEntity model = new ProviderEntity();
		protected ProviderEntitylFluentValidator providerValidator = new ProviderEntitylFluentValidator();

		[Inject] IProviderRepository providerRepository { get; set; }
		[Inject]
		NavigationManager navigationManager { get; set; }
		[Inject] public StateDetails? stateDetails { get; set; }

		[Parameter]
        public string? Id { get; set; }

        protected override async Task OnInitializedAsync()
		{
			if (Guid.TryParse(Id, out Guid provideId))
			{
				var providerToBeEdited = await providerRepository.GetProviderById(provideId);
				if (providerToBeEdited != null)
				{
					model = providerToBeEdited;
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
				
				if (await providerRepository.UpdateProvider(model))
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
