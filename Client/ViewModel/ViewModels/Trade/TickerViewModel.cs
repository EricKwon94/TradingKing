using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ViewModel.ViewModels.Trade;

public class TickerViewModel : ObservableObject
{
    public enum Change { Fall, Base, Rise }

    public event EventHandler<double>? PriceChanged;

    public string Code { get; set; }

    private double _price;
    public double Price
    {
        get => _price;
        set
        {
            if (value > _price)
                Change2 = Change.Rise;
            else if (value < _price)
                Change2 = Change.Fall;
            SetProperty(ref _price, value);
            PriceChanged?.Invoke(this, value);
        }
    }

    private Change _change;
    public Change Change2
    {
        get => _change;
        set => SetProperty(ref _change, value);
    }

    public TickerViewModel(string code, double price, Change change)
    {
        Code = code;
        _price = price;
        _change = change;
    }
}
