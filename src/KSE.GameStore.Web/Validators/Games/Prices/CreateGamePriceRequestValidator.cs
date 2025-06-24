using FluentValidation;
using KSE.GameStore.Web.Requests.Games;

namespace KSE.GameStore.Web.Validators.Games.Prices;

public class CreateGamePriceRequestValidator : AbstractValidator<CreateGamePriceRequest>
{
    public CreateGamePriceRequestValidator()
    {
        RuleFor(x => x.Value)
            .NotNull().WithMessage("Price is required")
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");
    }
}