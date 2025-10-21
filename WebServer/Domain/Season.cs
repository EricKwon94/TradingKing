using System;
using System.Collections.Generic;

namespace Domain;

public class Season : IEntity<int>
{
    public int Id { get; }
    public DateTime StartedAt { get; }

    public ICollection<Order> Orders { get; } = [];
}
