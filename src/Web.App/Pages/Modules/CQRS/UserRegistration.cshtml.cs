using Application.Abstractions.Messaging;
using Application.Users.RegisterUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedKernel;

namespace Web.App.Pages.Modules.CQRS;

/// <summary>
/// Demonstrates a CQRS entry point that dispatches a command through
/// ICommandDispatcher and the command behavior pipeline.
/// </summary>
public class UserRegistrationModel(ICommandDispatcher commandDispatcher) : PageModel
{
    [BindProperty]
    public string Email { get; set; } = "new.user@company.com";

    [BindProperty]
    public string FirstName { get; set; } = "Jane";

    [BindProperty]
    public string LastName { get; set; } = "Smith";

    [BindProperty]
    public string EmailTrigger { get; set; } = string.Empty;

    public bool HasResult { get; set; }
    public bool IsSuccess { get; set; }
    public Guid? UserId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ErrorLayer { get; set; }
    public bool IsValidationError { get; set; }
    public Error[] ValidationErrors { get; set; } = [];

    public void OnGet()
    {
        // Initial page load requires no work.
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            Email,
            FirstName,
            LastName,
            EmailTrigger);

        Result<Guid> result = await commandDispatcher.Send<RegisterUserCommand, Guid>(command, cancellationToken);

        HasResult = true;
        IsSuccess = result.IsSuccess;

        if (result.IsSuccess)
        {
            UserId = result.Value;
        }
        else if (result.Error is ValidationError validationError)
        {
            IsValidationError = true;
            ValidationErrors = validationError.Errors;
        }
        else
        {
            ErrorCode = result.Error.Code;
            ErrorDescription = result.Error.Description;
            ErrorLayer = ResolveErrorLayer(result.Error);
        }

        return Page();
    }

    private static string ResolveErrorLayer(Error error)
    {
        return error.Code switch
        {
            "Users.EmailProviderUnavailable" => "Infrastructure (External Provider - email provider)",
            "Users.EmailNotUnique" => "Infrastructure (Repository - uniqueness check)",
            _ => "Domain (Business Rules)"
        };
    }
}
