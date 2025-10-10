using System;

namespace Application.Services;

public interface IDispatcher
{
    void Invoke(Action action);
}
