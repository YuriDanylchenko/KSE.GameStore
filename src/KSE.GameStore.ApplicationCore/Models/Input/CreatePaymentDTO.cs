using KSE.GameStore.ApplicationCore.Models.Output;

namespace KSE.GameStore.ApplicationCore.Models.Input;

public record CreatePaymentDTO(
    int OrderId,
    PaymentMethod PaymentMethod
);