using System.Collections.Generic;

namespace ViewModel.Contracts;

public interface IQueryAttributable
{
    void ApplyQueryAttributes(IDictionary<string, object> query);
}
