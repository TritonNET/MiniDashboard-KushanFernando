using MiniDashboard.App.EntityModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MiniDashboard.App.Infrastructure.Converters
{
    public class StatusTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                StatusMessageType.Info => Brushes.LightBlue,
                StatusMessageType.Success => Brushes.LightGreen,
                StatusMessageType.Error => Brushes.IndianRed,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
