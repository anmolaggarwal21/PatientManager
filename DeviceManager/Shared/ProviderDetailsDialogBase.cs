using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Shared
{
    public class ProviderDetailsDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public string? Id { get; set; }
        protected void Cancel() => MudDialog.Cancel();

        [Inject]
        protected IProviderRepository providerRepository { get; set; }

        protected ProviderEntity? providerEntity { get; set; }

        protected async override Task OnInitializedAsync()
        {
            providerEntity = await providerRepository.GetProviderById(new Guid(Id!));
            await  base.OnInitializedAsync();
        }
    }
}
