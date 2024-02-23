using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace DeviceManager.Pages.User
{
    public class AddUserDetailsBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected RegisteredUserDto model = new RegisteredUserDto();
        protected RegisteredUserDtoFluentValidator registeredDtoValidator;
		protected IEnumerable<GenderEnum> genderTypes = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToList();
		protected IList<IdentityRole> roles = new List<IdentityRole>();
		[Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IUserRepository userRepository { get; set; }

		bool isShow, isShow1;
		public InputType PasswordInput = InputType.Password;
		public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

		public InputType PasswordInput1 = InputType.Password;
		public string PasswordInputIcon1 = Icons.Material.Filled.VisibilityOff;
		protected bool showUser { get; set; } = false;
		[Inject] ProtectedLocalStorage protectedLocalStorage { get; set; }
		protected void ButtonTestclick()
        {
			if (isShow)

            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }

            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

		protected void ButtonTestclick1()
		{
			if (isShow1)

			{
				isShow1 = false;
				PasswordInputIcon1 = Icons.Material.Filled.VisibilityOff;
				PasswordInput1 = InputType.Password;
			}

			else
			{
				isShow1 = true;
				PasswordInputIcon1 = Icons.Material.Filled.Visibility;
				PasswordInput1 = InputType.Text;
			}
		}



		protected async override Task OnInitializedAsync()
		{
			try
			{


				var roleName = await protectedLocalStorage.GetAsync<string?>("UserRole");
				if (roleName.Success && !string.IsNullOrEmpty(roleName.Value) && roleName.Value.Equals("Admin"))
				{
					showUser = true;
				}
				else
				{
					showUser = false;
				}
			}
			catch (Exception ex) { }
			if (showUser)
			{
				registeredDtoValidator = new RegisteredUserDtoFluentValidator(userRepository!);
				roles = await userRepository.GetRoles();
			}
			await base.OnInitializedAsync();
			
		}
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                if(await userRepository.GetUserByUsername(model.UserName) != null)
                {
					Snackbar.Add("User Name already exists");
					return;
				}
                if(! (await userRepository.AddUser(model)).Status)
                {
					Snackbar.Add("User Not Added!");

				}
                else
                {
					Snackbar.Add("User Added!");
					navigationManager.NavigateTo("/user");
				}

            }
        }

        protected void Cancel()
        {
            navigationManager.NavigateTo("/user");
        }
    }
}
