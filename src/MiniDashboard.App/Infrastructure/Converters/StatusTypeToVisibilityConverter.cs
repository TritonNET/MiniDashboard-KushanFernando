using MiniDashboard.App.EntityModels;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MiniDashboard.App.Infrastructure.Converters
{
    public class StatusTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StatusMessageType type)
                return type == StatusMessageType.None ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
