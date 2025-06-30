using FluentValidation;
using KSE.GameStore.Web.Requests.Genre;

namespace KSE.GameStore.Web.Validators.Genres;

public class CreateGenreRequestValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre name is required");

        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Genre name must be at least 2 characters long")
            .MaximumLength(50).WithMessage("Genre name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Genre name contains invalid characters.");
    }
}