using DeviceManager.Enums;
using DeviceManager.Repository;
using DeviceManager.Shared;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Color = MudBlazor.Color;

namespace DeviceManager.Pages.Dashboard
{
    public class DashboardDetailsBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        public List<ProviderEntity>? ProviderDto { get; set; }
        protected string searchString1 = "";
        protected ProviderEntity selectedItem1 = null;
        public HashSet<ProviderEntity> selectedItems = new HashSet<ProviderEntity>();
        protected MudTable<ProviderEntity> mudTable;
        DialogOptions disableBackdropClick = new DialogOptions() { DisableBackdropClick = true };
        protected bool pageLoaded { get; set; } = false;
        [Inject]
        protected IDialogService Dialog { get; set; }
        [Inject]
        protected IProviderRepository providerRespository { get; set; }
		[Inject]
		NavigationManager navigationManager { get; set; }
		public bool FilterFunc1(ProviderEntity element) => FilterFunc(element, searchString1);

        private bool FilterFunc(ProviderEntity element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.LegalName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.NPI.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            ProviderDto = new List<ProviderEntity>();
            ProviderDto.AddRange(await providerRespository.GetAllProvider());
            await base.OnInitializedAsync();
            pageLoaded = true;
        }

        protected void OnEditClick(Guid id)
        {
            var b = mudTable.SelectedItem;
            var a = selectedItem1;
			navigationManager.NavigateTo($"/EditProvider/{id}");
		}


        protected void OnDeleteClick(Guid id)
        {
            var parameters = new DialogParameters<DeleteDialog>();
            parameters.Add(x => x.ContentText, "Do you really want to delete these provider? This process cannot be undone.It is a cascading delete");
            parameters.Add(x => x.ButtonText, "Delete");
            parameters.Add(x => x.Color, Color.Error);
            parameters.Add(x => x.entityTypeEnum, EntityTypeEnum.Provider);
            parameters.Add(x => x.Id, id.ToString()); 

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

            Dialog.Show<DeleteDialog>("Delete", parameters, options);
        }
        protected void OnPatientClick(Guid id)
        {
            var legalName  = ProviderDto?.Find(x => x.ProviderId == id)?.LegalName;
            navigationManager.NavigateTo($"/Patient/{id}");
        }

        
    }
}
