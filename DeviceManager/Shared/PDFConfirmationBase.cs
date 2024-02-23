using DeviceManager.Enums;
using DeviceManager.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Color = MudBlazor.Color;

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
        [Parameter] public bool IsUser { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        IClipboardService ClipboardService { get; set; }
        protected async Task Submit()
        {
            if (!IsError && ShowCopy == false && !IsUser)
            {
                navigationManager.NavigateTo("/patient", true);
            }
            if (IsUser)
            {
                navigationManager.NavigateTo("/user", true);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }

        protected async Task CopyText()
        {
           await ClipboardService.CopyToClipboard(ContentText);
        }
        protected void Cancel() => MudDialog.Cancel();
        protected double startX, startY, offsetX, offsetY;

        protected void OnDragStart(DragEventArgs args)
        {
            startX = args.ClientX;
            startY = args.ClientY;
        }

        protected void OnDragEnd(DragEventArgs args)
        {
            offsetX += args.ClientX - startX;
            offsetY += args.ClientY - startY;
        }
    }
}
