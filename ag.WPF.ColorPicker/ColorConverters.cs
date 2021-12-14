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

    public class SBSLValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return Math.Round(System.Convert.ToDouble(value), 2, MidpointRounding.AwayFromZero) * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return System.Convert.ToDouble(value) / 100.0;
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

    public class OpaqueColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color color)) return null;
            return Color.FromRgb(color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PureColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return new HsbColor(System.Convert.ToDouble(value), 1.0, 1.0).ToRgbColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents class for inverting color of solid brush
    /// </summary>
    public class InvertColorConverter : IValueConverter
    {
        /// <summary>
        /// Inverts color of solid brush
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush brush)) return null;
            var color = brush.Color;
            return Color.FromArgb(255, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UpDownValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(System.Convert.ToDouble(value), MidpointRounding.AwayFromZero);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(System.Convert.ToDouble(value), MidpointRounding.AwayFromZero);
        }
    }

    public class ColorToBighterOrDarkerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var shadeColor = color.MakeBighterOrDarker((double)parameter);
            return new SolidColorBrush(shadeColor);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
