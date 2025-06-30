using KSE.GameStore.ApplicationCore.Models.Input;
using KSE.GameStore.ApplicationCore.Models.Output;

namespace KSE.GameStore.ApplicationCore.Services;

public interface IPaymentService
{
    /// <summary>
    /// Retrieves a paginated list of all payments.
    /// </summary>
    /// <param name="pageNumber">
    /// The 1-based page number to retrieve. If null, defaults to 1.
    /// </param>
    /// <param name="pageSize">
    /// The number of items per page. If null, defaults to 10.
    /// </param>
    /// <returns>
    /// A list of <see cref="PaymentDTO"/> representing the requested page of payments.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when either parameter is less than or equal to zero.
    /// </exception>
    Task<List<PaymentDTO>> GetAllPaymentsAsync(int? pageNumber, int? pageSize);
    
    /// <summary>
    /// Retrieves a payment by its unique identifier with all related entities.
    /// </summary>
    /// <param name="id">The unique identifier of the payment.</param>
    /// <returns>
    /// The complete <see cref="PaymentDTO"/> with the specified ID including order id,
    /// is payment confirmed field, datetime of payment, and payment method.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when id is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when no payment exists with the specified id.
    /// </exception>
    Task<PaymentDTO> GetPaymentByIdAsync(int id);
    
    /// <summary>
    /// Creates a new payment — the user pays for a specific order using the selected payment method.
    /// </summary>
    /// <param name="paymentDto">
    /// The <see cref="CreatePaymentDTO"/> containing:
    /// <list type="bullet">
    /// <item><description>The target order ID</description></item>
    /// <item><description>The selected payment method</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// An Excel invoice (file download) with:
    /// <list type="bullet">
    /// <item><description>Username</description></item>
    /// <item><description>Order ID</description></item>
    /// <item><description>Purchase date</description></item>
    /// <item><description>List of games with license keys</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>Required fields invalid</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>Order not found</description></item>
    /// <item><description>Payment method does not exist</description></item>
    /// </list>
    /// </exception>
    Task<byte[]> CreatePaymentAsync(CreatePaymentDTO paymentDto);
    
    /// <summary>
    /// Updates an existing payment.
    /// </summary>
    /// <param name="paymentDto">
    /// The <see cref="UpdatePaymentDTO"/> containing:
    /// <list type="bullet">
    /// <item><description>The Purchase date (optional)</description></item>
    /// <item><description>The selected payment method ID (optional)</description></item>
    /// </list>
    /// </param>
    /// <returns>
    /// The updated <see cref="PaymentDTO"/>.
    /// </returns>
    /// <exception cref="BadRequestException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>Required fields invalid</description></item>
    /// <item><description>Updated date is later than the current one</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>The payment to update doesn't exist</description></item>
    /// </list>
    /// </exception>
    Task<PaymentDTO> UpdatePaymentAsync(UpdatePaymentDTO paymentDto);
}