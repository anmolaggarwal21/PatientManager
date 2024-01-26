using Entities;
using FluentValidation;
using System.Linq;

namespace DeviceManager.Pages.Provider
{
    public class ProviderEntitylFluentValidator : AbstractValidator<ProviderEntity>
    {
        public ProviderEntitylFluentValidator()
        {
            RuleFor(x => x.LegalName)
                .NotEmpty();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Must(x=> {
                    if (x != null && x.StartsWith("0"))
                    {
                        return false;
                    }
                    return true;
                }).WithMessage("Phone Number cannot start with 0");

            RuleFor(x => x.Address)
               .NotEmpty();

            RuleFor(x => x.City)
               .NotEmpty();

            RuleFor(x => x.State)
               .NotEmpty();

            RuleFor(x => x.ZipCode)
               .NotEmpty()
               .ExclusiveBetween(10000,99999);

            RuleFor(x => x.NPI)
			   .ExclusiveBetween(1000000000, 9999999999);

			RuleFor(x => x.TaxId)
			 .ExclusiveBetween(100000000, 9999999999);

            RuleFor(x => x.ProviderNumber)
               .MaximumLength(6);
		}

        

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<ProviderEntity>.CreateWithOptions((ProviderEntity)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
