using FluentValidation;
using KSE.GameStore.Web.Requests.Publishers;

namespace KSE.GameStore.Web.Validators.Publishers;

public class UpdatePublisherRequestValidator : AbstractValidator<UpdatePublisherRequest>
{
    public UpdatePublisherRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Publisher ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Publisher name is required")
            .MaximumLength(100).WithMessage("Publisher name must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Publisher name contains invalid characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description must not be empty")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty().WithMessage("Website URL is required")
            .MaximumLength(200).WithMessage("Website URL must not exceed 200 characters")
            .Must(BeAValidUrl).WithMessage("Website URL must be a valid URL");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}