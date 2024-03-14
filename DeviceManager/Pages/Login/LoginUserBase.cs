using DeviceManager.Pages.User;
using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Nextended.Core.Extensions;
using System.Linq.Expressions;

namespace DeviceManager.Pages.Login
{
    public class LoginUserBase : ComponentBase
    {
        [Inject] ISnackbar Snackbar { get; set; }
        protected MudForm form;
        protected LoginUserDto model = new LoginUserDto();
        protected LoginUserFluentValidator loginUserFluentValidator;
        protected string ErrorMessage { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] IUserRepository userRepository { get; set; }
        [Inject] ProtectedLocalStorage protectedLocalStorage { get; set; }

        [Inject] IConfiguration configuration { get; set; }

		bool isShow, isShow1;
		public InputType PasswordInput = InputType.Password;
		public string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

		public InputType PasswordInput1 = InputType.Password;
		public string PasswordInputIcon1 = Icons.Material.Filled.VisibilityOff;
        protected bool pageLoaded { get; set; } = false;
        protected bool isError { get; set; } = false;

        protected string UserId { get; set; }


		protected bool IsResetPassword { get; set; } = false;
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

		


		protected async override Task OnInitializedAsync()
		{
            ErrorMessage = string.Empty;
			isError = false;
			loginUserFluentValidator = new LoginUserFluentValidator();
            pageLoaded = true;
            IsResetPassword = false;

			try
            {
               var isUserLoggedIn =  await protectedLocalStorage.GetAsync<bool>("UserLoggedIn");
                if (isUserLoggedIn.Success && isUserLoggedIn.Value)
                {
                    navigationManager.NavigateTo("/dashboard",forceLoad:true);
                }
            }
            catch(Exception ex)
            {

            }
            await base.OnInitializedAsync();
		}
		protected async Task Submit()
        {
            await form.Validate();

            if (form.IsValid)
            {
                pageLoaded = false;
                try
                {

                
                var result = await userRepository.AuthenticateUser(model);
                if (result.Status && result.User != null)
                {
                    ErrorMessage = string.Empty;
                    if (result.User.SecurityStamp.Equals(configuration.GetValue<string>("ConcurrencyStamp")))
                    {
                        UserId = result.User.Id;
                        IsResetPassword = true;

                    }
                    else
                    {
                        IsResetPassword = false;
                        var role = await userRepository.GetRoleOfUser(result.User);
                        await protectedLocalStorage.SetAsync("UserLoggedIn", true);
                        await protectedLocalStorage.SetAsync("UserDetails", result.User);
                        await protectedLocalStorage.SetAsync("UserRole", role);

                        await Task.Delay(1000);
                        navigationManager.NavigateTo("/dashboard", forceLoad: true);
                        Snackbar.Add("Login Successful");
                    }



                }
                else
                {
                    isError = true;
                    ErrorMessage = "  Invalid Username/Password";
                }
                }
                catch(Exception ex)
                {

                }
                pageLoaded = true;

            }
        }

       
    }
}
