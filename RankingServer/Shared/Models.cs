using System;

namespace Shared;

public record OrderModel(Guid Id, int SeasonId, string UserId, string Code, double Quantity, double Price);
public record SeasonModel(int Id, DateTime StartedAt);
public record UserModel(string Id, string Password);
public record RankModel(int SeasonId, string UserId, double Assets);