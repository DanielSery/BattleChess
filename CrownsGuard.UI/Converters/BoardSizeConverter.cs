using System.Globalization;
using System.Windows.Data;

namespace CrownsGuard.UI.Converters;

public class BoardSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is [double width, double height])
        {
            if (height >= width - 300)
            {
                return width - 300;
            }

            if (height <= width)
            {
                return height;
            }
            
            return width;
        }
        return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}