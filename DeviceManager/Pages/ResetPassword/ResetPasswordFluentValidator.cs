using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using FluentValidation;
using System.Linq;

namespace DeviceManager.Pages.ResetPassword
{
    public class ResetPasswordFluentValidator : AbstractValidator<ResetPasswordDto>
    {
        
		public ResetPasswordFluentValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Username)
                .NotEmpty();
			RuleFor(x => x.NewPassword)
			 .NotEmpty()
			 .MustAsync(async (parent, child, context, token) =>
			 {
				 if (!string.IsNullOrEmpty(child) && !string.IsNullOrWhiteSpace(child))
				 {
					 var result = await userRepository.PasswordVerification(child);
					 if (result != null)
					 {
						 context.AddFailure(result);
					 }
				 }

				 return true;
			 });

			RuleFor(x => x.NewReenterPassword)
		 .NotEmpty()
		 .Must((parent, child, context) =>
		 {
			 if (!string.IsNullOrEmpty(child) && !string.IsNullOrWhiteSpace(child))
			 {
				 if (!child.Equals(parent.NewPassword))

				 {
					 context.AddFailure("New Password and Re enter Password do not match");
				 }
			 }

			 return true;
		 });

		}
		
		public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ResetPasswordDto>.CreateWithOptions((ResetPasswordDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
