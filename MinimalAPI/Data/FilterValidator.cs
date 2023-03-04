using FluentValidation;
using MinimalAPI.Api.Enums;

namespace MinimalAPI.Api.Data
{
    public class FilterValidator : AbstractValidator<KeyValuePair<FilterKey, string>>
    {
        public FilterValidator()
        {
            RuleFor(filter => filter.Value)
                .NotEmpty()
                .Must(value => int.TryParse(value, out _))
                .When(filter => filter.Key == FilterKey.PurchaseType)
                .WithMessage("Invalid PurchaseType filter value.");
        }
    }
}
