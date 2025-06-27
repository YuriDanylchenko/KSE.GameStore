namespace KSE.GameStore.Web.Requests.Payments;

public record UpdatePaymentRequest(
    int Id,
    DateTime PayedAt,
    int PaymentMethod
);