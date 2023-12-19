using Entities;
using FluentValidation;

namespace DeviceManager.Pages.Patient
{
    public class PatientEntitylFluentValidator : AbstractValidator<PatientEntity>
    {
        public PatientEntitylFluentValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty();

			RuleFor(x => x.PhoneNumber)
				.NotEmpty()
				.Must(x => {
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

            RuleFor(x => x.state)
               .NotEmpty();

            RuleFor(x => x.ZipCode)
               .NotEmpty()
               .ExclusiveBetween(10000, 99999);

            RuleFor(x => x.Gender)
               .NotNull();

            RuleFor(x => x.AdmissionDate)

                .Must(x =>
                {
                    if (!x.HasValue || x.GetValueOrDefault() == DateTime.MinValue)
                        return false;

                    return true;
                })
              .WithMessage("Admission date must not be empty ")

              .Must(x => 
              {
                  if(x.GetValueOrDefault() <= DateTime.Now)
                  return true;
                 
                  return false;
              } )
              .WithMessage("Admission date cannot be greater than today's date ");

            RuleFor(x => x.DOB)

               .Must(x =>
               {
                   if (!x.HasValue || x.GetValueOrDefault() == DateTime.MinValue)
                       return false;

                   return true;
               })
             .WithMessage("Date Of Birth must not be empty ")

             .Must(x =>
             {
                 if (x.GetValueOrDefault() <= DateTime.Now)
                     return true;

                 return false;
             })
             .WithMessage("Date Of Birth cannot be greater than today's date ");

			RuleFor(x => x.Provider)
	          .Must(x => {
		          if (x == null || x.LegalName == null)
			          return false;
		          return true;
	          })
	          .WithMessage("Please select Provider");
		}



        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PatientEntity>.CreateWithOptions((PatientEntity)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
