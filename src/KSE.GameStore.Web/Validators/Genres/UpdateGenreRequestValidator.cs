using FluentValidation;
using KSE.GameStore.Web.Requests.Genre;

namespace KSE.GameStore.Web.Validators.Genres;

public class UpdateGenreRequestValidator : AbstractValidator<UpdateGenreRequest>
{
    public UpdateGenreRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Genre ID must be greater than 0");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre name is required");
            
        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Genre name must be at least 2 characters long")
            .MaximumLength(50).WithMessage("Genre name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Genre name contains invalid characters");
    }
}