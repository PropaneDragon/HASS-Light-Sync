using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LightSync.UI.Converters
{
    public class BoolToValueConverter : IValueConverter
    {
        public object? TrueValue { get; set; }
        public object? FalseValue { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool booleanValue)
                return new BindingNotification(new InvalidCastException("Cannot convert from a value which is not a boolean."), BindingErrorType.Error);

            return booleanValue ? TrueValue : FalseValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value == TrueValue;
        }
    }
}
