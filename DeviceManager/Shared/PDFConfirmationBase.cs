using DeviceManager.Enums;
using DeviceManager.Repository;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Shared
{
    public class PDFConfirmationBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string ContentText { get; set; }

        [Parameter] public string ButtonText { get; set; }

        [Parameter] public Color Color { get; set; }

        [Parameter] public bool IsError { get; set; }

        [Parameter] public bool ShowCopy { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        IClipboardService ClipboardService { get; set; }
        protected async Task Submit()
        {
            if (!IsError && ShowCopy == false)
            {
                navigationManager.NavigateTo("/patient", true);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        protected async Task CopyText()
        {
           await ClipboardService.CopyToClipboard(ContentText);
        }
        protected void Cancel() => MudDialog.Cancel();
    }
}
