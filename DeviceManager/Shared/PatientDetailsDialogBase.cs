using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Shared
{
    public class PatientDetailsDialogBase: ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public PatientEntity? patientEntity { get; set; }
        protected void Cancel() => MudDialog.Cancel();

        

       
    }
}
