using DeviceManager.Repository;
using Entities;
using Entities.UserManagement.Models;
using FluentValidation;
using System.Linq;

namespace DeviceManager.Pages.User
{
    public class RegisteredUserDtoFluentValidator : AbstractValidator<RegisteredUserDto>
    {
        
		public RegisteredUserDtoFluentValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                 .MaximumLength(50);

            RuleFor(x => x.UserName)
               .NotEmpty()
               .MinimumLength(5)
               .MaximumLength(50)
               .MustAsync( async (parent, child, context, token) => {
                   if (!string.IsNullOrEmpty(child) && !string.IsNullOrWhiteSpace(child))
                   {
                       var user = await userRepository.GetUserByUsername(child);
                       if (user != null)
                       {
                           context.AddFailure("Username already present");
                       }
                   }
                  
                   return true;
               });
            RuleFor(x => x.Gender)
               .NotEmpty();
			RuleFor(x => x.Password)
			 .NotEmpty()
             .MustAsync( async (parent, child,context, token) => 
             {
                 if(!string.IsNullOrEmpty(child) && !string.IsNullOrWhiteSpace(child))
                 {
                     var result = await userRepository.PasswordVerification(child);
                     if (result != null)
                     {
                         context.AddFailure(result);
                     }
                 }
                 
				 return true;
             });

			RuleFor(x => x.ReEnterPassword)
		 .NotEmpty()
		 .Must( (parent, child, context) =>
		 {
			 if (!string.IsNullOrEmpty(child) && !string.IsNullOrWhiteSpace(child))
			 {
				 if(!child.Equals(parent.Password))
				 
				 {
					 context.AddFailure("Password and Re enter Password do not match");
				 }
			 }

			 return true;
		 });

			RuleFor(x=> x.Role).NotEmpty().NotNull();

		}

        

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<RegisteredUserDto>.CreateWithOptions((RegisteredUserDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
