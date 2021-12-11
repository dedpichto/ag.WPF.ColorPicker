using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    /// <summary>
    /// Structure to define HSB.
    /// </summary>
    public struct HsbColor
    {
        /// <summary>
        /// Gets an empty HSB structure;
        /// </summary>
        public static readonly HsbColor Empty = new HsbColor();
        private double hue;
        private double saturation;
        private double brightness;
        //public static bool operator ==(HSB iteml, HSB item2)
        //{
        // return (
        // iteml.Hue == item2.Hue
        // && iteml.Saturation == item2.Saturation
        // && iteml.Brightness == item2.Brightness
        // ) j
        //) 
        //public static bool operator !=(HSB iteml, HSB item2)
        //4
        // return ( 
        // iteml.Hue != item2.Hue
        // || item1l.Saturation != item2.Saturation
        // i | iteml.Brightness != item2.Brightness
        // );
        //}
        /// <summary> |
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

        public static bool operator ==(HsbColor item1, HsbColor item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Brightness == item2.Brightness
                );
        }

        public static bool operator !=(HsbColor item1, HsbColor item2)
        {
            return !(item1 == item2);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            return (this == (HsbColor)obj);
        }

        public override int GetHashCode()
        {
            return (Hue.GetHashCode(), Saturation.GetHashCode(), Brightness.GetHashCode()).GetHashCode();
        }

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
    }
}