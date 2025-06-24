using KSE.GameStore.ApplicationCore.Models.Output;

namespace KSE.GameStore.ApplicationCore.Models.Input;

public record UpdatePaymentDTO(
    int Id,
    DateTime? PayedAt,
    PaymentMethodDTO? PaymentMethod
);