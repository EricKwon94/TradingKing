using CommunityToolkit.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Globalization;
using ViewModel.ViewModels.Trade;

namespace View.Converters;

public class TickerChangeToConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TickerViewModel.Change.Rise)
            return Colors.Red;
        else if (value is TickerViewModel.Change.Fall)
            return Colors.Blue;
        return new AppThemeColor
        {
            Default = Colors.Black,
            Light = Colors.Black,
            Dark = Colors.White,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
