using System;

namespace Shared;

public record OrderModel(Guid Id, int SeasonId, string UserId, string Code, double Quantity, double Price);