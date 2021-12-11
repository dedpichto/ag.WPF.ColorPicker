using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    internal static class Utils
    {
        public static readonly Dictionary<string, Color> KnownColors = GetKnownColors();

        public static string GetColorName(this Color color)
        {
            string str = Utils.KnownColors.Where(kvp => kvp.Value.Equals(color)).Select(kvp => kvp.Key).FirstOrDefault();
            if (string.IsNullOrEmpty(str))
                str = color.ToString();
            return str;
        }

        public static string FormatColorString(string stringToFormat, bool isUseAlphaChannel) => !isUseAlphaChannel && stringToFormat.Length == 9 ? stringToFormat.Remove(1, 2) : stringToFormat;

        private static Dictionary<string, Color> GetKnownColors() => ((IEnumerable<PropertyInfo>)typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)).ToDictionary(p => p.Name, p => (Color)p.GetValue(null, null));

        //public static HsvColor ConvertRgbToHsv(int r, int g, int b)
        //{
        //    double num1 = 0.0;
        //    double num2 = Math.Min(Math.Min(r, g), b);
        //    double num3 = Math.Max(Math.Max(r, g), b);
        //    double num4 = num3 - num2;
        //    double num5 = num3 != 0.0 ? num4 / num3 : 0.0;
        //    double num6;
        //    if (num5 == 0.0)
        //    {
        //        num6 = 0.0;
        //    }
        //    else
        //    {
        //        if (r == num3)
        //            num1 = (g - b) / num4;
        //        else if (g == num3)
        //            num1 = 2.0 + (b - r) / num4;
        //        else if (b == num3)
        //            num1 = 4.0 + (r - g) / num4;
        //        num6 = num1 * 60.0;
        //        if (num6 < 0.0)
        //            num6 += 360.0;
        //    }
        //    return new HsvColor()
        //    {
        //        H = num6,
        //        S = num5,
        //        V = num3 / byte.MaxValue
        //    };
        //}

        public static Color ToRgbColor(this HsbColor hsb)
        {
            double num1;
            double num2;
            double num3;
            if (hsb.Saturation == 0.0)
            {
                num1 = hsb.Brightness;
                num2 = hsb.Brightness;
                num3 = hsb.Brightness;
            }
            else
            {
                var h = hsb.Hue;
                if (h == 360.0)
                    h = 0.0;
                else
                    h /= 60.0;
                int num4 = (int)Math.Truncate(h);
                double num5 = h - num4;
                double num6 = hsb.Brightness * (1.0 - hsb.Saturation);
                double num7 = hsb.Brightness * (1.0 - hsb.Saturation * num5);
                double num8 = hsb.Brightness * (1.0 - hsb.Saturation * (1.0 - num5));
                switch (num4)
                {
                    case 0:
                        num1 = hsb.Brightness;
                        num2 = num8;
                        num3 = num6;
                        break;
                    case 1:
                        num1 = num7;
                        num2 = hsb.Brightness;
                        num3 = num6;
                        break;
                    case 2:
                        num1 = num6;
                        num2 = hsb.Brightness;
                        num3 = num8;
                        break;
                    case 3:
                        num1 = num6;
                        num2 = num7;
                        num3 = hsb.Brightness;
                        break;
                    case 4:
                        num1 = num8;
                        num2 = num6;
                        num3 = hsb.Brightness;
                        break;
                    default:
                        num1 = hsb.Brightness;
                        num2 = num6;
                        num3 = num7;
                        break;
                }
            }
            return Color.FromArgb(byte.MaxValue, (byte)Math.Round(num1 * byte.MaxValue), (byte)Math.Round(num2 * byte.MaxValue), (byte)Math.Round(num3 * byte.MaxValue));
        }

        public static double ConvertHsbToDouble(Color color)
        {
            var hsv = color.ToHsbColor();
            return 360.0 - hsv.Hue;
        }

        public static List<Color> GenerateHsvPalette()
        {
            var colorList = new List<Color>();
            int num = 60;
            HsbColor hsb;
            for (int index = 0; index < 360; index += num)
            {
                hsb = new HsbColor(index, 1.0, 1.0);
                colorList.Add(hsb.ToRgbColor());
            }
            hsb = new HsbColor(0.0, 1.0, 1.0);
            colorList.Add(hsb.ToRgbColor());
            colorList.Reverse();
            return colorList;
        }

        //public static Color ToRgbColor(this HsbColor hsb)
        //{
        //    double r = 0;
        //    double g = 0;
        //    double b = 0;
        //    if (hsb.Saturation == 0)
        //    {
        //        r = g = b = hsb.Brightness;
        //    }
        //    else
        //    {
        //        // the color wheel consists of 6 sectors. Figure out whith sector
        //        // you're in.
        //        double sectorPos = hsb.Hue / 60.0;
        //        int sectorNumber = (int)Math.Floor(sectorPos);
        //        // get the fractional part of the sector
        //        double fractionalSector = sectorPos - sectorNumber;
        //        // calculate values for the three axes of the color.
        //        double p = hsb.Brightness * (1.0 - hsb.Saturation);
        //        double q = hsb.Brightness * (1.0 - (hsb.Saturation * fractionalSector));
        //        double t = hsb.Brightness * (1.0 - (hsb.Saturation * (1 - fractionalSector)));
        //        // assign the fractional colors to r, g, and b based on Ehe sector
        //        // the angle is in.
        //        switch (sectorNumber)
        //        {
        //            case 0:
        //                r = hsb.Brightness;
        //                g = t;
        //                b = p;
        //                break;
        //            case 1:
        //                r = q; ;
        //                g = hsb.Brightness;
        //                b = p;
        //                break;
        //            case 2:
        //                r = p;
        //                g = hsb.Brightness;
        //                b = t;
        //                break;
        //            case 3:
        //                r = p;
        //                g = q;
        //                b = hsb.Brightness;
        //                break;
        //            case 4:
        //                r = t;
        //                g = p;
        //                b = hsb.Brightness;
        //                break;
        //            case 5:
        //                r = hsb.Brightness;
        //                g = p;
        //                b = q;
        //                break;
        //        }
        //    }
        //    return Color.FromArgb(byte.MaxValue,
        //    Convert.ToByte(Double.Parse(String.Format("{0:0.00}", r * 255.0))),
        //    Convert.ToByte(Double.Parse(String.Format("{0:0.00}", g * 255.0))),
        //    Convert.ToByte(Double.Parse(String.Format("{0:0.00}", b * 255.0)))
        //    );
        //}

        public static Color ToRgbColor(this HslColor hsl)
        {
            if (hsl.Saturation == 0)
            {
                // achromatic color’ (gray scale}
                return Color.FromArgb(byte.MaxValue,
                    (byte)Math.Round(hsl.Luminance * 255.0, MidpointRounding.AwayFromZero),
                (byte)Math.Round(hsl.Luminance * 255.0,
                MidpointRounding.AwayFromZero),
                (byte)Math.Round(hsl.Luminance * 255.0,
                MidpointRounding.AwayFromZero)
                );
            }
            else
            {
                {
                    double q = (hsl.Luminance < 0.5) ? (hsl.Luminance * (1.0 +
                    hsl.Saturation)) : (hsl.Luminance + hsl.Saturation - (hsl.Luminance * hsl.Saturation));
                    double p = (2.0 * hsl.Luminance) - q;
                    double Hk = hsl.Hue / 360.0;
                    double[] T = new double[3];
                    T[0] = Hk + (1.0 / 3.0); // Tr
                    T[1] = Hk; // Th .
                    T[2] = Hk - (1.0 / 3.0); // Tg
                    for (int i = 0; i < 3; i++)
                    {
                        if (T[i] < 0) T[i] += 1.0;
                        if (T[i] > 1) T[i] -= 1.0;
                        if ((T[i] * 6) < 1)
                        {
                            T[i] = p + ((q - p) * 6.0 * T[i]);
                        }
                        else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i] 40.5
                        {
                            T[i] = q;
                        }
                        else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
                        {
                            T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
                        }
                        else T[i] = p;
                    }
                    //return new RGB (
                    // Convert .ToInt32 (Double. Parse (String. Format ("{0:0.0d}", :
                    // T[O}] * 255.0))),
                    // Convert .ToInt32 (Double. Parse (String. Format ("{0:0.00}",
                    // T{1] * 255.0))),
                    // Convert .ToInt32 (Double. Parse (String. Format ("{0:0.0d}",
                    // T[2] * 255.0)))
                    // i
                    return Color.FromArgb(byte.MaxValue,
                    (byte)Math.Round(T[0] * 255.0, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(T[1] * 255.0, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(T[2] * 255.0, MidpointRounding.AwayFromZero)
                    );
                }
            }
        }

        public static HslColor ToHslColor(this Color color)
        {
            double h = 0, s = 0, l = 0;

            // normalize red, green, blue values
            double r = (double)color.R / 255.0;
            double g = (double)color.G / 255.0;
            double b = (double)color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));

            // hue
            if (max == min)
            {
                h = 0; // undefined
            }
            else if (max == r && g >= b)
            {
                h = 60.0 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60.0 * (g - b) / (max - min) + 360.0;
            }
            else if (max == g)
            {
                h = 60.0 * (b - r) / (max - min) + 120.0;
            }
            else if (max == b)
            {
                h = 60.0 * (r - g) / (max - min) + 240.0;
            }

            // luminance
            l = (max + min) / 2.0;

            // saturation
            if (l == 0 || max == min)
            {
                s = 0;
            }
            else if (0 < l && l <= 0.5)
            {
                s = (max - min) / (max + min);
            }
            else if (l > 0.5)
            {
                s = (max - min) / (2 - (max + min)); //(max-min > 0)?
            }

            //return new HSL (
            // Double. Parse (String. Format("{0:0.00}", h)),
            // Double. Parse (String. Format ("{0:0.00}", s)),
            // Double. Parse (String.Format("{0:0.00}", l))
            // ); |
            return new HslColor(h, s, l);
        }

        public static HsbColor ToHsbColor(this Color color)
        {
            // normalize red, green and blue values
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;
            // conversion start
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double h = 0.0;
            if (max == r && g >= b)
            {
                h =max==min?0: 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }
            double s = (max == 0) ? 0.0 : (1.0 - (min / max));
            return new HsbColor(h, s, (double)max);
        }

        public static Color GetBighterOrDarker(this Color color, double factor)
        {
            if(factor<-1)throw new ArgumentException("Factor must bew greater equal -1",nameof(factor));
            if (factor > 1) throw new ArgumentException("Factor mast be smaller equal 1", nameof(factor));
            if (factor == 0) return color;
            if (factor < 0)
            {
                factor += 1;
                return Color.FromArgb(color.A,(byte)(color.R*factor), (byte)(color.G*factor), (byte)(color.B*factor));
            }
            else
            {
                return Color.FromArgb(color.A,
                    (byte)(color.R + (byte.MaxValue - color.R) * factor),
                    (byte)(color.G + (byte.MaxValue - color.G) * factor),
                    (byte)(color.B + (byte.MaxValue - color.B) * factor));
            }
        }
    }
}
