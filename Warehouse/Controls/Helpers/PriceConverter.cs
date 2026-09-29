using System;
using System.Globalization;
using System.Windows.Data;
using WarehouseData.Models;

namespace Warehouse.Controls.Helpers
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product product)
            {
                return product.Price.ToString("N2", new CultureInfo("ru-RU")) + " руб.";
            }
            if (value is decimal d)
            {
                return d.ToString("N2", new CultureInfo("ru-RU")) + " руб.";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}