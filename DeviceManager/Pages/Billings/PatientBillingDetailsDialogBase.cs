using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Pages.Billings
{
    public class PatientBillingDetailsDialogBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public BillingEntityDto? billingDetails { get; set; }
        protected void Cancel() => MudDialog.Cancel();

        

       
    }
}
