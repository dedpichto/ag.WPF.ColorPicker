using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.ColorPicker
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double newValue)) return Colors.Red;
            var hsb = new HsbColor(360.0 - newValue, 1.0, 1.0);
            return hsb.ToRgbColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color)) return 0.0;
            return Utils.ConvertHsbToDouble(color);
        }
    }

    public class HueValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double hue)) return null;
            return Math.Round(hue, MidpointRounding.AwayFromZero);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SliderThumbVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3
                || !(values[0] is double maxValue)
                || !(values[1] is double minValue)
                || !(values[2] is double currentValue)
                || !(parameter is string position)) return Visibility.Collapsed;

            switch (position)
            {
                case "up":
                    return currentValue == maxValue ? Visibility.Visible : (object)Visibility.Collapsed;
                case "down":
                    return currentValue == minValue ? Visibility.Visible : (object)Visibility.Collapsed;
                default:
                    return currentValue < maxValue && currentValue > minValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
