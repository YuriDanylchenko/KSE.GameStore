using FluentValidation;
using KSE.GameStore.Web.Requests.Games;
using KSE.GameStore.Web.Validators.Games.Prices;

namespace KSE.GameStore.Web.Validators.Games.Games;

public class UpdateGameRequestValidator : AbstractValidator<UpdateGameRequest>
{
    public UpdateGameRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Game ID must be greater than 0");
        
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");
            
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Title contains invalid characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description must not be empty")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters")
            .When(x => x.Description != null);
        
        RuleFor(x => x.PublisherId)
            .GreaterThan(0).WithMessage("Publisher ID must be greater than 0")
            .WithMessage("Publisher ID is required");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("At least one genre is required")
            .ForEach(id => id.GreaterThan(0).WithMessage("Genre ID must be greater than 0"));
        
        RuleFor(x => x.PlatformIds)
            .NotEmpty().WithMessage("At least one platform is required")
            .ForEach(id => id.GreaterThan(0).WithMessage("Platform ID must be greater than 0"));
        
        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price information is required")
            .SetValidator(new UpdateGamePriceRequestValidator());
        
        RuleFor(x => x.RegionPermissionIds)
            .ForEach(id => id.GreaterThan(0))
            .When(x => x.RegionPermissionIds != null)
            .WithMessage("Region permission ID must be greater than 0 if provided");
    }
}