using AutoMapper;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Services;
using KSE.GameStore.Web.Requests.Payments;
using Microsoft.AspNetCore.Mvc;

namespace KSE.GameStore.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;

    public PaymentsController(IPaymentService paymentService, IMapper mapper)
    {
        _paymentService = paymentService;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest createPaymentRequest)
    {
        var paymentDto = _mapper.Map<CreatePaymentRequest, CreatePaymentDTO>(createPaymentRequest);
        var fileBytes = await _paymentService.CreatePaymentAsync(paymentDto);

        var fileName = $"invoice-order-{paymentDto.OrderId}.xlsx";

        return File(
            fileContents: fileBytes,
            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileDownloadName: fileName
        );
    }
}