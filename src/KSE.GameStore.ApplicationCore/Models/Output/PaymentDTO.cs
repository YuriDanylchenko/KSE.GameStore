namespace KSE.GameStore.ApplicationCore.Models.Output;

public record PaymentDTO(
    int Id,
    int OrderId,
    bool Confirmed,
    DateTime PayedAt,
    PaymentMethodDTO? PaymentMethod
);