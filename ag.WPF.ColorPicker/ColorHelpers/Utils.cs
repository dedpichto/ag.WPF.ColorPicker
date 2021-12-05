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

        public static HsvColor ConvertRgbToHsv(int r, int g, int b)
        {
            double num1 = 0.0;
            double num2 = Math.Min(Math.Min(r, g), b);
            double num3 = Math.Max(Math.Max(r, g), b);
            double num4 = num3 - num2;
            double num5 = num3 != 0.0 ? num4 / num3 : 0.0;
            double num6;
            if (num5 == 0.0)
            {
                num6 = 0.0;
            }
            else
            {
                if (r == num3)
                    num1 = (g - b) / num4;
                else if (g == num3)
                    num1 = 2.0 + (b - r) / num4;
                else if (b == num3)
                    num1 = 4.0 + (r - g) / num4;
                num6 = num1 * 60.0;
                if (num6 < 0.0)
                    num6 += 360.0;
            }
            return new HsvColor()
            {
                H = num6,
                S = num5,
                V = num3 / byte.MaxValue
            };
        }

        public static Color ConvertHsvToRgb(double h, double s, double v)
        {
            double num1;
            double num2;
            double num3;
            if (s == 0.0)
            {
                num1 = v;
                num2 = v;
                num3 = v;
            }
            else
            {
                if (h == 360.0)
                    h = 0.0;
                else
                    h /= 60.0;
                int num4 = (int)Math.Truncate(h);
                double num5 = h - num4;
                double num6 = v * (1.0 - s);
                double num7 = v * (1.0 - s * num5);
                double num8 = v * (1.0 - s * (1.0 - num5));
                switch (num4)
                {
                    case 0:
                        num1 = v;
                        num2 = num8;
                        num3 = num6;
                        break;
                    case 1:
                        num1 = num7;
                        num2 = v;
                        num3 = num6;
                        break;
                    case 2:
                        num1 = num6;
                        num2 = v;
                        num3 = num8;
                        break;
                    case 3:
                        num1 = num6;
                        num2 = num7;
                        num3 = v;
                        break;
                    case 4:
                        num1 = num8;
                        num2 = num6;
                        num3 = v;
                        break;
                    default:
                        num1 = v;
                        num2 = num6;
                        num3 = num7;
                        break;
                }
            }
            return Color.FromArgb(byte.MaxValue, (byte)Math.Round(num1 * byte.MaxValue), (byte)Math.Round(num2 * byte.MaxValue), (byte)Math.Round(num3 * byte.MaxValue));
        }

        public static double ConvertRgbToDouble(Color color)
        {
            HsvColor hsv = ConvertRgbToHsv(color.R, color.G, color.B);
            return 360.0 - hsv.H;
        }

        public static List<Color> GenerateHsvPalette()
        {
            var colorList = new List<Color>();
            int num = 60;
            for (int index = 0; index < 360; index += num)
                colorList.Add(ConvertHsvToRgb((double)index, 1.0, 1.0));
            colorList.Add(ConvertHsvToRgb(0.0, 1.0, 1.0));
            return colorList;
        }
    }
}
