using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    /// <summary>
    /// Represents HSB color structure.
    /// </summary>
    public struct HsbColor
    {
#nullable disable
        /// <summary>
        /// Gets an empty HSB structure.
        /// </summary>
        public static readonly HsbColor Empty = new();
        private double hue;
        private double saturation;
        private double brightness;

        /// <summary>
        /// Gets or sets the hue component.
        /// </summary> 
        public double Hue
        {
            get
            {
                return hue;
            }
            set
            {
                hue = (value > 360) ? 360 : ((value < 0) ? 0 : value);
            }
        }
        /// <summary>
        /// Gets or sets saturation component.
        /// </summary>
        public double Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                saturation = (value > 1) ? 1 : ((value < 0) ? 0 : value);
            }
        }
        /// <summary>
        /// Gets or sets the brightness component.
        /// </summary>
        public double Brightness
        {
            get
            {
                return brightness;
            }
            set
            {
                brightness = (value > 1) ? 1 : ((value < 0) ? 0 : value);
            }
        }
        /// <summary>
        /// Creates an instance of a HSB structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="b">Brightness value.</param>
        public HsbColor(double h, double s, double b)
        {
            hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            brightness = (b > 1) ? 1 : ((b < 0) ? 0 : b);
        }

        /// <summary>
        /// Compares two HSB structures.
        /// </summary>
        /// <param name="item1">First structure.</param>
        /// <param name="item2">Second structure.</param>
        /// <returns>True if structures are equal.</returns>
        public static bool operator ==(HsbColor item1, HsbColor item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Brightness == item2.Brightness
                );
        }

        /// <summary>
        /// Compares two HSB structures.
        /// </summary>
        /// <param name="item1">First structure.</param>
        /// <param name="item2">Second structure.</param>
        /// <returns>True if structures are inequal.</returns>
        public static bool operator !=(HsbColor item1, HsbColor item2)
        {
            return !(item1 == item2);
        }

        /// <summary>
        /// Compares HSB structure with another object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if HSL structure is equal to object.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return (this == (HsbColor)obj);
        }

        /// <summary>
        /// Gets HSL color hash code.
        /// </summary>
        /// <returns>HSB color hash code.</returns>
        public override int GetHashCode() => (Hue, Saturation, Brightness).GetHashCode();

        /// <summary>
        /// Converts HSB color to RGB color.
        /// </summary>
        /// <returns>RGB color.</returns>
        public Color ToRgbColor()
        {
            double num1;
            double num2;
            double num3;
            if (Saturation == 0.0)
            {
                num1 = Brightness;
                num2 = Brightness;
                num3 = Brightness;
            }
            else
            {
                var h = Hue;
                if (h == 360.0)
                    h = 0.0;
                else
                    h /= 60.0;
                int num4 = (int)Math.Truncate(h);
                double num5 = h - num4;
                double num6 = Brightness * (1.0 - Saturation);
                double num7 = Brightness * (1.0 - Saturation * num5);
                double num8 = Brightness * (1.0 - Saturation * (1.0 - num5));
                switch (num4)
                {
                    case 0:
                        num1 = Brightness;
                        num2 = num8;
                        num3 = num6;
                        break;
                    case 1:
                        num1 = num7;
                        num2 = Brightness;
                        num3 = num6;
                        break;
                    case 2:
                        num1 = num6;
                        num2 = Brightness;
                        num3 = num8;
                        break;
                    case 3:
                        num1 = num6;
                        num2 = num7;
                        num3 = Brightness;
                        break;
                    case 4:
                        num1 = num8;
                        num2 = num6;
                        num3 = Brightness;
                        break;
                    default:
                        num1 = Brightness;
                        num2 = num6;
                        num3 = num7;
                        break;
                }
            }
            return Color.FromArgb(byte.MaxValue, (byte)Math.Round(num1 * byte.MaxValue), (byte)Math.Round(num2 * byte.MaxValue), (byte)Math.Round(num3 * byte.MaxValue));
        }
#nullable restore
    }
}