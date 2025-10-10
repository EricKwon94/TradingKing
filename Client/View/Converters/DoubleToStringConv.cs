using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace View.Converters;

internal class DoubleToStringConv : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double price = System.Convert.ToDouble(value);
        string result = price % 1 == 0
            ? price.ToString("N0")
            : price.ToString("N8").TrimEnd('0').TrimEnd('.');

        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
