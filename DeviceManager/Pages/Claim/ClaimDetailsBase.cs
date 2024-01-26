
using DeviceManager.Enums;
using DeviceManager.Pages.Provider;
using DeviceManager.Repository;
using DeviceManager.Shared;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Color = MudBlazor.Color;

namespace DeviceManager.Pages.Claim

{
    public class ClaimDetailsBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        public List<ClaimEntity>? ClaimDto { get; set; }
        protected string searchString1 = "";
        protected ClaimEntity selectedItem1 = null;
        public HashSet<ClaimEntity> selectedItems = new HashSet<ClaimEntity>();
        protected MudTable<ClaimEntity> mudTable;
        DialogOptions disableBackdropClick = new DialogOptions() { DisableBackdropClick = true };
        protected bool pageLoaded { get; set; } = false;
        [Inject]
        protected IDialogService Dialog { get; set; }
        [Inject]
        protected IClaimRepository claimRepository { get; set; }
        [Inject]
        protected IPatientRepository patientRepository { get; set; }
        [Inject]
		NavigationManager navigationManager { get; set; }

        [Parameter]
        public string? patientId { get; set; }

        public string? headerValue { get; set; } = "Claim Details";

        public bool FilterFunc1(ClaimEntity element) => FilterFunc(element, searchString1);

        private bool FilterFunc(ClaimEntity element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            if (element.Patient.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            ClaimDto = new List<ClaimEntity>();
            if (string.IsNullOrEmpty(patientId) || !Guid.TryParse(patientId, out Guid guidPatientId))
            {
                headerValue = "";
                ClaimDto.AddRange(await claimRepository.GetAllClaim());
            }
            else
            {
              var patient =   await patientRepository.GetPatientById(guidPatientId);
                headerValue = $"Patient ({patient?.FullName})";
                ClaimDto.AddRange(await claimRepository.GetClaimByPatientId(guidPatientId));
            }
            await base.OnInitializedAsync();
            pageLoaded = true;
        }

        protected void OnEditClick(Guid id)
        {
            var b = mudTable.SelectedItem;
            var a = selectedItem1;
			navigationManager.NavigateTo($"/EditClaim/{id}");
		}


        protected void OnDeleteClick(Guid id)
        {
            var parameters = new DialogParameters<DeleteDialog>();
            parameters.Add(x => x.ContentText, "Do you really want to delete these claim? This process cannot be undone.");
            parameters.Add(x => x.ButtonText, "Delete");
            parameters.Add(x => x.Color, Color.Error);
            parameters.Add(x => x.entityTypeEnum, EntityTypeEnum.Claim);
            parameters.Add(x => x.Id, id.ToString()); 

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

            Dialog.Show<DeleteDialog>("Delete", parameters, options);
        }
       

    }
}
