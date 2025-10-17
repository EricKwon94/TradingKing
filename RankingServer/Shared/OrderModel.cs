using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Shared;

public record OrderModel(Guid Id, string UserId, string Code, double Quantity, double Price)
{
    public class Comparer : IEqualityComparer<OrderModel>
    {
        public bool Equals(OrderModel? x, OrderModel? y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode([DisallowNull] OrderModel obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}

