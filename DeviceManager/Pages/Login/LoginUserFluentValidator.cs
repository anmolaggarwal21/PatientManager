using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using FluentValidation;
using System.Linq;

namespace DeviceManager.Pages.Login
{
    public class LoginUserFluentValidator : AbstractValidator<LoginUserDto>
    {
        
		public LoginUserFluentValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty();
			RuleFor(x => x.Password)
			  .NotEmpty();

		}

        

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<LoginUserDto>.CreateWithOptions((LoginUserDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
