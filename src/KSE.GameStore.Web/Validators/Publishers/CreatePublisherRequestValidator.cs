using FluentValidation;
using KSE.GameStore.Web.Requests.Publishers;

namespace KSE.GameStore.Web.Validators.Publishers;

public class CreatePublisherRequestValidator : AbstractValidator<CreatePublisherRequest>
{
    public CreatePublisherRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Publisher name is required");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Publisher name must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-':,\.]+$")
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Publisher name contains invalid characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description must not be empty")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.WebsiteUrl)
            .NotEmpty().WithMessage("Website URL is required")
            .MaximumLength(200).WithMessage("Website URL must not exceed 200 characters")
            .Must(BeAValidUrl).WithMessage("Website URL must be a valid URL")
            .When(x => x.WebsiteUrl != null);
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}