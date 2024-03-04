using DeviceManager.Pages.Provider;
using DeviceManager.Repository;
using DeviceManager.Shared;
using Entities;
using Entities.UserManagement.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using MudBlazor;

namespace DeviceManager.Pages.User
{
	public class EditUserDetailsBase : ComponentBase
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
        [Inject]
        protected IDialogService Dialog { get; set; }
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

		[Parameter]
		public string? UserId { get; set; }

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
				if (UserId != null)
				{
					var userToBeEdited = await userRepository.GetUserById(UserId);
					if (userToBeEdited != null)
					{
						if (userToBeEdited.NormalizedUserName.Equals("SUPERUSER"))
						{
							ShowErrorDialog("Cannot edit Superuser");
							return;
						}
						var loggedInUserLocalStorage = await protectedLocalStorage.GetAsync<Users>("UserDetails");
						if (loggedInUserLocalStorage.Success && loggedInUserLocalStorage.Value != null)
						{
							if (loggedInUserLocalStorage.Value.Id.Equals(userToBeEdited.Id))
							{
                                ShowErrorDialog("Cannot edit Self");
								return;
							}

						}
						RegisteredUserDto registeredUserDto = new RegisteredUserDto()
						{
							FirstName = userToBeEdited.FirstName,
							//Gender = providerToBeEdited.Gender,
							LastName = userToBeEdited.LastName,
							UserName = userToBeEdited.UserName,

						};
						if (Enum.TryParse(userToBeEdited.Gender, out GenderEnum myStatus))
						{
							registeredUserDto.Gender = myStatus;
						}
						else
						{
							registeredUserDto.Gender = GenderEnum.Male;

						}
						var userRole = await userRepository.GetRoleOfUser(userToBeEdited);
						registeredUserDto.Role = roles.First(x => x.Name.Equals(userRole));
						model = registeredUserDto;
					}
				}

			}
			await base.OnInitializedAsync();

		}
		protected async Task Submit()
		{
			await form.Validate();

			if (form.IsValid)
			{
				if (await userRepository.GetUserByUsername(model.UserName) == null)
				{
					Snackbar.Add("User Does not exist");
				}
				if (!(await userRepository.UpdateUser(model)))
				{
					Snackbar.Add("User Not Update!");

				}
				else
				{
					Snackbar.Add("User Updated!");
					navigationManager.NavigateTo("/user");
				}

			}
		}

		protected void Cancel()
		{
			navigationManager.NavigateTo("/user");

		}


		protected async Task ResetPassword()
		{

            var userToBeEdited = await userRepository.GetUserById(UserId!);
			if(userToBeEdited !=  null)
            {
                var usernameCasing = userToBeEdited.UserName.Substring(0, 1).ToUpper() + userToBeEdited.UserName.Substring(1);
                if(await userRepository.UpdateUserPassword(userToBeEdited, $"{usernameCasing}1234567!", true))
				{
					ShowErrorDialog($"Password Reset with password as {usernameCasing}1234567!", IsError: false,Header: "Success" );

                }
				else
				{
                    ShowErrorDialog($"Password Not Reset");

                }


            }

        }

		private void ShowErrorDialog(string message, bool IsError = true, string Header ="Error")
		{

			var parameters = new DialogParameters<PDFConfirmation>();
			parameters.Add(x => x.ContentText, $"{message}");
			parameters.Add(x => x.ButtonText, "Ok");
		
			parameters.Add(x => x.IsError, IsError);
            parameters.Add(x => x.IsUser, true);
            parameters.Add(x => x.ShowCopy, false);
			if (IsError)
			{
                parameters.Add(x => x.Color, MudBlazor.Color.Error);
			}
			else
			{
                parameters.Add(x => x.Color, MudBlazor.Color.Success);
            }
			var options = new DialogOptions();
			options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, DisableBackdropClick = true };
			Dialog.Show<PDFConfirmation>(Header, parameters, options);
		}
	}
}
