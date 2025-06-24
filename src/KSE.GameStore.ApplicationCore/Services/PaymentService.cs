using AutoMapper;
using ClosedXML.Excel;
using KSE.GameStore.ApplicationCore.Infrastructure;
using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;
using KSE.GameStore.DataAccess.Entities;
using KSE.GameStore.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace KSE.GameStore.ApplicationCore.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment, int> _paymentRepository;
    private readonly IRepository<Order, int> _orderRepository;
    private readonly ILogger<PaymentService> _logger;
    private readonly IMapper _mapper;

    public PaymentService(
        IRepository<Payment, int> paymentRepository,
        IRepository<Order, int> orderRepository,
        // IRepository<UserGameStock, (Guid, int)> userGameStockRepo,
        ILogger<PaymentService> logger,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<PaymentDTO>> GetAllPaymentsAsync(int? pageNumber, int? pageSize)
    {
        if (pageNumber is <= 0)
            throw new BadRequestException($"Page number must be a positive integer. Provided: {pageNumber}");

        if (pageSize is <= 0)
            throw new BadRequestException($"Page size must be a positive integer. Provided: {pageSize}");

        var paymentEntities = await _paymentRepository.ListAsync(pageNumber ?? 1, pageSize ?? 10);

        return _mapper.Map<List<PaymentDTO>>(paymentEntities);
    }

    public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
    {
        var paymentEntity = await _paymentRepository.GetByIdAsync(id);

        if (paymentEntity == null)
        {
            _logger.LogNotFound($"payment/{id}");
            throw new NotFoundException($"Payment with ID {id} not found.");
        }

        return _mapper.Map<PaymentDTO>(paymentEntity);
    }

    public async Task<byte[]> CreatePaymentAsync(CreatePaymentDTO paymentDto)
    {
        var orderEntity = await _orderRepository.GetByIdAsync(paymentDto.OrderId);
        
        if (orderEntity == null)
        {
            throw new NotFoundException($"Order with ID {paymentDto.OrderId} not found.");
        }
        
        // TODO: check regions permissions
        
        var payment = _mapper.Map<Payment>(paymentDto);
        payment.Confirmed = true;
        payment.PayedAt = DateTime.UtcNow;
        payment.Order = orderEntity;

        orderEntity.Payment = payment;
        orderEntity.UpdatedAt = DateTime.UtcNow;
        orderEntity.Status = OrderStatus.Payed;
        
        await _paymentRepository.AddAsync(payment);
        _orderRepository.Update(orderEntity);
        await _paymentRepository.SaveChangesAsync();
        
        // generate license keys for each game
        var licenseKeys = new Dictionary<string, string>(); // Game Title -> License Key
        foreach (var item in orderEntity.OrderItems)
        {
            licenseKeys[item.Game.Title] = GenerateLicenseKey(item.Game.Id, orderEntity.UserId);
        }

        // return Excel
        using var workbook = CreateExcelFile(orderEntity, licenseKeys);
        
        // TODO: add to the db table "user-game-stock"

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<PaymentDTO> UpdatePaymentAsync(UpdatePaymentDTO paymentDto)
    {
        var paymentEntity = await _paymentRepository.GetByIdAsync(paymentDto.Id);

        if (paymentEntity == null)
        {
            _logger.LogNotFound($"payment/{paymentDto.Id}");
            throw new NotFoundException($"Payment with ID {paymentDto.Id} not found.");
        }

        if (paymentDto.PayedAt > DateTime.UtcNow)
        {
            throw new BadRequestException("The date is in the future.");
        }

        // Update all fields
        _mapper.Map(paymentDto, paymentEntity);

        _paymentRepository.Update(paymentEntity);
        await _paymentRepository.SaveChangesAsync();

        return _mapper.Map<PaymentDTO>(await _paymentRepository.GetByIdAsync(paymentDto.Id));
    }

    private XLWorkbook CreateExcelFile(Order order, Dictionary<string, string> licenseKeys)
    {
        var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Invoice");

        var row = 1;
        sheet.Cell(row++, 1).Value = "User Name:";
        sheet.Cell(row - 1, 2).Value = order.User.Name;

        sheet.Cell(row++, 1).Value = "Order ID:";
        sheet.Cell(row - 1, 2).Value = order.Id;

        sheet.Cell(row++, 1).Value = "Purchase Date:";
        sheet.Cell(row - 1, 2).Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        row++; // spacer
        sheet.Cell(row++, 1).Value = "Game";
        sheet.Cell(row - 1, 2).Value = "License Key";

        foreach (var kv in licenseKeys)
        {
            sheet.Cell(row, 1).Value = kv.Key;
            sheet.Cell(row, 2).Value = kv.Value;
            row++;
        }

        return workbook;
    }
    
    private string GenerateLicenseKey(int gameId, Guid userId)
    {
        return $"LIC-{gameId}-{userId}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}