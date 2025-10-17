using System;

namespace Shared;

public record OrderModel(Guid Id, string UserId, string Code, double Quantity, double Price);