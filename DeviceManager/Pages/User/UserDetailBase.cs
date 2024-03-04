using DeviceManager.Enums;
using DeviceManager.Repository;
using DeviceManager.Shared;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using Color = MudBlazor.Color;

namespace DeviceManager.Pages.User
{
    public class UserDetailBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        public List<DisplayUser>? UsersDto { get; set; }
        protected string searchString1 = "";
        protected DisplayUser selectedItem1 = null;
        public HashSet<DisplayUser> selectedItems = new HashSet<DisplayUser>();
        protected MudTable<DisplayUser> mudTable;
        protected bool showUser { get; set; } = false;
        DialogOptions disableBackdropClick = new DialogOptions() { DisableBackdropClick = true };
        protected bool pageLoaded { get; set; } = false;
        [Inject]
        protected IDialogService Dialog { get; set; }
        [Inject]
        protected IUserRepository userRepository { get; set; }
		[Inject]
		NavigationManager navigationManager { get; set; }
      
        [Inject] ProtectedLocalStorage protectedLocalStorage { get; set; }
        public bool FilterFunc1(DisplayUser element) => FilterFunc(element, searchString1);

        private bool FilterFunc(DisplayUser element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (!string.IsNullOrEmpty(element.FirstName) && element.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrEmpty(element.UserName) && element.UserName.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            
			if (!string.IsNullOrEmpty (element.LastName) && element.LastName.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var roles = await protectedLocalStorage.GetAsync<string?>("UserRole");
                if (roles.Success && !string.IsNullOrEmpty(roles.Value) && roles.Value.Equals("Admin"))
                {
                    showUser = true;
                }
                else
                {
                    showUser = false;
                }
            }
           catch (Exception ex)
            {

            }
            if (showUser)
            {


                UsersDto = new List<DisplayUser>();
               
                var allUser = await userRepository.GetAllUser();
                if (allUser != null)
                {
                    foreach (var user in allUser)
                    {
                        var roleNameOfUser = await userRepository.GetRoleOfUser(user);
                        UsersDto.Add(new DisplayUser
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            UserName = user.UserName,
                            Gender = user.Gender,
                            RoleName = roleNameOfUser,
                            Id = user.Id
                        });
                    }
                };
            }
            pageLoaded = true;
            await base.OnInitializedAsync();
		}

		

		protected void OnEditClick(string id)
        {
            var b = mudTable.SelectedItem;
            var a = selectedItem1;
			navigationManager.NavigateTo($"/EditUser/{id}");
		}


        protected void OnDeleteClick(string id)
        {
            var parameters = new DialogParameters<DeleteDialog>();
            parameters.Add(x => x.ContentText, "Do you really want to delete these User");
            parameters.Add(x => x.ButtonText, "Delete");
            parameters.Add(x => x.Color, Color.Error);
            parameters.Add(x => x.entityTypeEnum, EntityTypeEnum.User);
            parameters.Add(x => x.Id, id.ToString()); 

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

            Dialog.Show<DeleteDialog>("Delete", parameters, options);
        }

	}
}
