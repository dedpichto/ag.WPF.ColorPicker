﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    internal static class Utils
    {
#nullable disable
        public static readonly HashSet<Type> NumericTypes = new()
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        public static readonly Dictionary<string, (Color, HsbColor)> KnownColors = GetKnownColors();

        private static Dictionary<string, (Color, HsbColor)> GetKnownColors() => ((IEnumerable<PropertyInfo>)typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)).ToDictionary(p => p.Name, p => ((Color)p.GetValue(null, null), ((Color)p.GetValue(null, null)).ToHsbColor()));

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

        public static HslColor ToHslColor(this Color color)
        {
            double hue = 0, sat = 0;

            // normalize red, green, blue values
            double red = (double)color.R / 255.0;
            double green = (double)color.G / 255.0;
            double blue = (double)color.B / 255.0;

            double max = Math.Max(red, Math.Max(green, blue));
            double min = Math.Min(red, Math.Min(green, blue));

            // hue
            if (max == min)
            {
                hue = 0; // undefined
            }
            else if (max == red && green >= blue)
            {
                hue = 60.0 * (green - blue) / (max - min);
            }
            else if (max == red && green < blue)
            {
                hue = 60.0 * (green - blue) / (max - min) + 360.0;
            }
            else if (max == green)
            {
                hue = 60.0 * (blue - red) / (max - min) + 120.0;
            }
            else if (max == blue)
            {
                hue = 60.0 * (red - green) / (max - min) + 240.0;
            }

            // luminance
            double lum = (max + min) / 2.0;

            // saturation
            if (lum == 0 || max == min)
            {
                sat = 0;
            }
            else if (0 < lum && lum <= 0.5)
            {
                sat = (max - min) / (max + min);
            }
            else if (lum > 0.5)
            {
                sat = (max - min) / (2 - (max + min)); //(max-min > 0)?
            }

            return new HslColor(hue, sat, lum);
        }

        public static HsbColor ToHsbColor(this Color color)
        {
            // normalize red, green and blue values
            double red = color.R / 255.0;
            double green = color.G / 255.0;
            double blue = color.B / 255.0;
            // conversion start
            double max = Math.Max(red, Math.Max(green, blue));
            double min = Math.Min(red, Math.Min(green, blue));
            double hue = 0.0;
            if (max == red && green >= blue)
            {
                hue = max == min ? 0 : 60 * (green - blue) / (max - min);
            }
            else if (max == red && green < blue)
            {
                hue = 60 * (green - blue) / (max - min) + 360;
            }
            else if (max == green)
            {
                hue = 60 * (blue - red) / (max - min) + 120;
            }
            else if (max == blue)
            {
                hue = 60 * (red - green) / (max - min) + 240;
            }
            double sat = (max == 0) ? 0.0 : (1.0 - (min / max));
            return new HsbColor(hue, sat, (double)max);
        }

        public static Color MakeBighterOrDarker(this Color color, double factor)
        {
            if (factor < -1) throw new ArgumentException("Factor must bew greater equal -1", nameof(factor));
            if (factor > 1) throw new ArgumentException("Factor mast be smaller equal 1", nameof(factor));
            if (factor == 0) return color;
            if (factor < 0)
            {
                factor += 1;
                return Color.FromArgb(color.A, (byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
            }
            else
            {
                return Color.FromArgb(color.A,
                    (byte)(color.R + (byte.MaxValue - color.R) * factor),
                    (byte)(color.G + (byte.MaxValue - color.G) * factor),
                    (byte)(color.B + (byte.MaxValue - color.B) * factor));
            }
        }

        public static DrawingBrush TransparentBrush()
        {
            var geometryGroup = new GeometryGroup();
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, 50, 50)));
            geometryGroup.Children.Add(new RectangleGeometry(new Rect(50, 50, 50, 50)));
            var geometryDrawingGray = new GeometryDrawing { Brush = Brushes.LightGray, Geometry = geometryGroup };
            var geometryDrawingWhile = new GeometryDrawing { Brush = Brushes.White, Geometry = new RectangleGeometry(new Rect(0, 0, 100, 100)) };
            var drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(geometryDrawingWhile);
            drawingGroup.Children.Add(geometryDrawingGray);
            var drawingBrush = new DrawingBrush { TileMode = TileMode.Tile, Viewport = new Rect(0, 0, 10, 10), ViewportUnits = BrushMappingMode.Absolute, Drawing = drawingGroup };
            return drawingBrush;
        }

        public static bool In<T>(this T value, params T[] values) => values.Contains(value);
#nullable restore
    }
}
