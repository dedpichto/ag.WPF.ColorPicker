using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.ColorPicker
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double newValue)) return Colors.Red;
            return Utils.ConvertHsvToRgb(360.0 - newValue, 1.0, 1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color)) return 0.0;
            return Utils.ConvertRgbToDouble(color);
        }
    }

    public class NullableColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;
            if (color == null || !color.HasValue) return Brushes.Black;
            return new SolidColorBrush(Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? new SolidColorBrush((Color)value) : value;
        }
        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return value != null ? (object)((SolidColorBrush)value).Color : value;
        }
    }
}
