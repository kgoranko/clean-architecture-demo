using Application.Orders.Abstractions;
using Application.Orders.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedKernel;

namespace Web.App.Pages.Modules.DirectDi;

/// <summary>
/// OrderProcessingModel demonstrates Approach 1: Direct DI Service Call.
///
/// The Razor Page (Presentation layer) injects IOrderProcessingService
/// and calls its method directly. This is the simplest approach to
/// invoke Application layer logic from the UI.
/// </summary>
public class OrderProcessingModel(IOrderProcessingService orderProcessingService) : PageModel
{
    #region Form Inputs
    [BindProperty]
    public string CustomerName { get; set; } = "John Doe";

    [BindProperty]
    public string ProductName { get; set; } = "Laptop";

    [BindProperty]
    public int Quantity { get; set; } = 2;

    [BindProperty]
    public string PaymentTrigger { get; set; } = string.Empty;
    #endregion

    #region Result Display
    public bool HasResult { get; set; }
    public bool IsSuccess { get; set; }
    public new OrderProcessingResponse? Response { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ErrorLayer { get; set; }
    #endregion

    public void OnGet()
    {
        // Initial page load - no processing needed
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════
        // APPROACH 1: Direct DI Service Call
        //
        // The Razor Page simply creates a request DTO and calls the
        // application service method. The service handles all orchestration.
        // This is as simple as it gets - inject and call.
        // ═══════════════════════════════════════════════════════════════
        var request = new OrderProcessingRequest(
            CustomerName,
            ProductName,
            Quantity,
            PaymentTrigger);

        Result<OrderProcessingResponse> result = await orderProcessingService.ProcessOrderAsync(
            request, cancellationToken);

        HasResult = true;
        IsSuccess = result.IsSuccess;

        if (result.IsSuccess)
        {
            Response = result.Value;
        }
        else
        {
            ErrorCode = result.Error.Code;
            ErrorDescription = result.Error.Description;
            ErrorLayer = result.Error.Type switch
            {
                ErrorType.NotFound => "Infrastructure (Repository)",
                ErrorType.Problem => "Domain (Business Rules)",
                ErrorType.Failure => "Infrastructure (External Service)",
                _ => "Unknown"
            };
        }

        return Page();
    }
}
