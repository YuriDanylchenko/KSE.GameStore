namespace KSE.GameStore.Web.Requests.Payments;

public record CreatePaymentRequest(
    int OrderId,
    int PaymentMethod
);