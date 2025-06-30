using FluentValidation;
using KSE.GameStore.Web.Requests.Platforms;

namespace KSE.GameStore.Web.Validators.Platforms;

public class CreatePlatformRequestValidator : AbstractValidator<CreatePlatformRequest>
{
    public CreatePlatformRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Platform name is required");

        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Platform name must be at least 2 characters long")
            .MaximumLength(50).WithMessage("Platform name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Platform name contains invalid characters");
    }
}