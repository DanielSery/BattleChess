using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BattleChess3.UI.Converters;

public class IsStringEmptyMultiValueConverter : MarkupExtension, IValueConverter
{
    public object? TrueValue { get; set; }
    public object? FalseValue { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is string str && string.IsNullOrEmpty(str)
            ? TrueValue
            : FalseValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}