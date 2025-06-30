using System;
using System.Collections.Generic;

namespace KSE.GameStore.ApplicationCore.Models.Output;

public record OrderDTO(
    int Id,
    Guid UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string Status,
    List<OrderItemDTO> OrderItems
);