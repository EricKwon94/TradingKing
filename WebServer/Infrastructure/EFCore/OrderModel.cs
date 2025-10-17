using System;

namespace Infrastructure.EFCore;

public record OrderModel(Guid Id, string UserId, string Code, double Quantity, double Price);