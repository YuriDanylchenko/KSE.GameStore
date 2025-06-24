using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDTO paymentDto)
    {
        var fileBytes = await _paymentService.CreatePaymentAsync(paymentDto);

        var fileName = $"invoice-order-{paymentDto.OrderId}.xlsx";

        return File(
            fileContents: fileBytes,
            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDownloadName: fileName
        );
    }
}