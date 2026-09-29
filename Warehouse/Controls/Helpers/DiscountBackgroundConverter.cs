using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using WarehouseData.Models;

namespace Warehouse.Controls.Helpers
{
    public class DiscountBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product product)
            {
                if (product.DiscountPercent > 0)
                    return new SolidColorBrush(Color.FromRgb(255, 240, 160));
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}