using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SupportTicketClient;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value as string;
        return status switch
        {
            "Open" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28A745")), // Green
            "In Progress" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")), // Yellow
            "Closed" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6C757D")), // Gray
            _ => Brushes.Transparent
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}