using DeviceManager.Pages.User;
using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Nextended.Core.Extensions;
using OneOf.Types;

namespace DeviceManager.Pages.ResetPassword
{
    public class ResetPasswordBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected ResetPasswordDto model = new ResetPasswordDto();
        protected ResetPasswordFluentValidator resetPasswordFluentValidator;
        protected string ErrorMessage { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IUserRepository userRepository { get; set; }
        [Inject] ProtectedLocalStorage protectedLocalStorage { get; set; }

        [Inject] IConfiguration configuration { get; set; }

		[Parameter]
		public string? UserId { get; set; }

		bool isShow, isShow1;
		public InputType PasswordInput = InputType.Password;
		public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

		public InputType PasswordInput1 = InputType.Password;
		public string PasswordInputIcon1 = Icons.Material.Filled.VisibilityOff;
        protected bool pageLoaded { get; set; } = false;
        protected bool isError { get; set; } = false;
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
            ErrorMessage = string.Empty;
			isError = false;
			resetPasswordFluentValidator = new ResetPasswordFluentValidator(userRepository);
            pageLoaded = true;
            if(UserId != null)
            {
				var userToBeEdited = await userRepository.GetUserById(UserId);
                if(userToBeEdited != null)
                {
                    model.Username = userToBeEdited.UserName;
                }
			}
			await base.OnInitializedAsync();
        }
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid && UserId != null)
            {
				pageLoaded = false;
				var userToBeEdited = await userRepository.GetUserById(UserId!);
                if (userToBeEdited != null)
                {
                   var isPasswordChanged = await userRepository.UpdateUserPassword(userToBeEdited, model.NewPassword, false);
                    if (isPasswordChanged)
					{

						var role = await userRepository.GetRoleOfUser(userToBeEdited);
						await protectedLocalStorage.SetAsync("UserLoggedIn", true);
						await protectedLocalStorage.SetAsync("UserDetails", userToBeEdited);
						await protectedLocalStorage.SetAsync("UserRole", role);

						await Task.Delay(1000);
						navigationManager.NavigateTo("/dashboard", forceLoad: true);
						Snackbar.Add("Password Reset Successful");
					}
                }
				pageLoaded = true;

            }
        }

       
    }
}
