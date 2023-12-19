using Entities;
using FluentValidation;

namespace DeviceManager.Pages.Claim
{
	public class ClaimEntitylFluentValidator : AbstractValidator<ClaimEntity>
	{
        public ClaimEntitylFluentValidator()
        {
            RuleFor(x => x.Provider)
            .Must(x => {
                if (x == null || x.LegalName == null)
                    return false;
                return true;
            })
            .WithMessage("Please select Provider");

			RuleFor(x => x.Patient)
				.Must((parent, child, context)=> {
					if(child == null || child.FullName == null)
						return false;
					return true;
				})
				.WithMessage("Please select Patient");
          

			RuleFor(x => x.RevenueCode)
			.NotEmpty();
			RuleFor(x => x.CPTCode)
			.NotEmpty();
			RuleFor(x => x.CoveredUnits)
			.NotEmpty();
			RuleFor(x => x.NonCoveredUnits)
			.NotEmpty();
			RuleFor(x => x.CoveredCharges)
			.NotEmpty();
			RuleFor(x => x.CoveredCharges)
			.NotEmpty();
			RuleFor(x => x.TotalCharges)
			.NotEmpty();
			RuleFor(x => x.ServiceDate)

			  .Must(x =>
			  {
				  if (!x.HasValue || x.GetValueOrDefault() == DateTime.MinValue)
					  return false;

				  return true;
			  })
			.WithMessage("Service Date must not be empty ")

			.Must(x =>
			{
				if (x.GetValueOrDefault() <= DateTime.Now)
					return true;

				return false;
			})
			.WithMessage("Service Date cannot be greater than today's date ");
		}
        

		public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
		{
			var result = await ValidateAsync(ValidationContext<ClaimEntity>.CreateWithOptions((ClaimEntity)model, x => x.IncludeProperties(propertyName)));
			if (result.IsValid)
				return Array.Empty<string>();
			return result.Errors.Select(e => e.ErrorMessage);
		};
	}
}
