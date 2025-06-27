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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPaymentById([FromRoute] int id)
    {
        return Ok(await _paymentService.GetPaymentByIdAsync(id));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPayments(int? pageNumber, int? pageSize)
    {
        var payments = await _paymentService.GetAllPaymentsAsync(pageNumber, pageSize);
        return Ok(payments);
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
    
    [HttpPut]
    public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentRequest updatePaymentRequest)
    {
        var paymentDto = _mapper.Map<UpdatePaymentRequest, UpdatePaymentDTO>(updatePaymentRequest);
        var changedPayment = await _paymentService.UpdatePaymentAsync(paymentDto);

        return Ok(changedPayment);
    }
}