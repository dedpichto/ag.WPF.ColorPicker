﻿using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ag.WPF.ColorPicker
{
#nullable disable
    /// <summary>
    /// Multiplies/divides saturation/brightness/luminance by 100.
    /// </summary>
    public class SBSLValueConverter : IValueConverter
    {
        /// <summary>
        /// Multiplies saturation/brightness/luminance by 100.
        /// </summary>
        /// <param name="value">Saturation/brightness/luminance.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Double.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return Math.Round(System.Convert.ToDouble(value), 2, MidpointRounding.AwayFromZero) * 100;
        }

        /// <summary>
        ///Ddivides saturation/brightness/luminance by 100
        /// </summary>
        /// <param name="value">Saturation/brightness/luminance.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Double.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return System.Convert.ToDouble(value) / 100.0;
        }
    }

    /// <summary>
    /// Converts color to opaque, ignoring A value.
    /// </summary>
    public class OpaqueColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts color to opaque, ignoring A value.
        /// </summary>
        /// <param name="value">Color.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Color</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color color) return null;
            return Color.FromRgb(color.R, color.G, color.B);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns color based on Hue value only.
    /// </summary>
    public class PureColorConverter : IValueConverter
    {
        /// <summary>
        /// Returns color based on Hue value only.
        /// </summary>
        /// <param name="value">Color.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>HSBColor.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Utils.NumericTypes.Contains(value.GetType())) return null;
            return new HsbColor(System.Convert.ToDouble(value), 1.0, 1.0).ToRgbColor();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns HEX representation of color.
    /// </summary>
    public class ColorToHexStringConverter : IValueConverter
    {
        /// <summary>
        /// Returns HEX representation of color.
        /// </summary>
        /// <param name="value">Color.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>String.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color color) return null;
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Returns inverted color.
    /// </summary>
    public class InvertColorByColorConverter : IValueConverter
    {
        /// <summary>
        /// Returns inverted color.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine(value);
            if (value is not Color color)
                return null;
            return Color.FromArgb(255, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents class for inverting color of solid brush.
    /// </summary>
    public class InvertColorByBrushConverter : IValueConverter
    {
        /// <summary>
        /// Inverts color of solid brush.
        /// </summary>
        /// <param name="value">Solid brush.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush brush)
                return null;
            var color = brush.Color;
            return Color.FromArgb(255, (byte)~color.R, (byte)~color.G, (byte)~color.B);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Rounds value from/for UpDown.
    /// </summary>
    public class UpDownValueConverter : IValueConverter
    {
        /// <summary>
        /// Rounds value from UpDown.
        /// </summary>
        /// <param name="value">Decimal value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Double.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(System.Convert.ToDouble(value), MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds value for UpDown.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Double.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(System.Convert.ToDouble(value), MidpointRounding.AwayFromZero);
        }
    }

    /// <summary>
    /// Makes color brighter/darker.
    /// </summary>
    public class ColorToBighterOrDarkerConverter : IValueConverter
    {
        /// <summary>
        /// Makes color brighter/darker.
        /// </summary>
        /// <param name="value">Color.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Solid brush.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            var shadeColor = color.MakeBighterOrDarker((double)parameter);
            return new SolidColorBrush(shadeColor);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts byte value to and from HEX string.
    /// </summary>
    public class ARGBHexStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts byte value to HEX string.
        /// </summary>
        /// <param name="value">Byte.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>String.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte b)
            {
                var result = $"{b:X2}";
                if (b != 0 && result.StartsWith("0"))
                    result = result.Substring(1);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Converts HEX string to byte value.
        /// </summary>
        /// <param name="value">String.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Byte.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return !string.IsNullOrEmpty(s) ? System.Convert.ToByte($"0x{s}", 16) : 0;
            return null;
        }
    }

    /// <summary>
    /// Converts byte value to and from HEX string.
    /// </summary>
    public class ARGBStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts byte value to byte string.
        /// </summary>
        /// <param name="value">Byte.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>String.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte b)
                return b.ToString();
            return null;
        }

        /// <summary>
        /// Converts byte string to byte value.
        /// </summary>
        /// <param name="value">String.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Byte.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            if (value is string s)
                result = !string.IsNullOrEmpty(s) ? System.Convert.ToInt32(s) : 0;
            if (result > byte.MaxValue) result = byte.MaxValue;
            return (byte)result;
        }
    }
#nullable restore
}
