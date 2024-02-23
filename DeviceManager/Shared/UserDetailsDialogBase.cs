using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Shared
{
    public class UserDetailsDialogBase: ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public Users? UsersEntity { get; set; }

        [Parameter]
        public string? RoleName { get; set; }
        protected void Cancel() => MudDialog.Cancel();

        

       
    }
}
