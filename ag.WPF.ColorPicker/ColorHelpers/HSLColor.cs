using System;
using System.Windows.Media;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    /// <summary>
    /// Represents HSL color structure.
    /// </summary>
    public struct HslColor
    {
#nullable disable
        /// <summary>
        /// Gets an empty HSL structure.
        /// </summary>
        public static readonly HslColor Empty = new();

        private double hue;
        private double saturation;
        private double luminance;

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
        /// Gets or sets the luminance component.
        /// </summary>
        public double Luminance
        {
            get
            {
                return luminance;
            }
            set
            {
                luminance = (value > 1) ? 1 : ((value < 0) ? 0 : value);
            }
        }

        /// <summary>
        /// Creates an instance of a HSL structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="l">Lightness value.</param>
        public HslColor(double h, double s, double l)
        {
            hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
        }

        /// <summary>
        /// Compares two HSL structures.
        /// </summary>
        /// <param name="item1">First structure.</param>
        /// <param name="item2">Second structure.</param>
        /// <returns>True if structures are equal.</returns>
        public static bool operator ==(HslColor item1, HslColor item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Luminance == item2.Luminance
                );
        }

        /// <summary>
        /// Compares two HSL structures.
        /// </summary>
        /// <param name="item1">First structure.</param>
        /// <param name="item2">Second structure.</param>
        /// <returns>True if structures are inequal.</returns>
        public static bool operator !=(HslColor item1, HslColor item2)
        {
            return !(item1 == item2);
        }

        /// <summary>
        /// Compares HSL structure with another object.
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>True if HSL structure is equal to object.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HslColor)obj);
        }

        /// <summary>
        /// Gets HSL color hash code.
        /// </summary>
        /// <returns>HSL color hash code.</returns>
        public override int GetHashCode() => (Hue, Saturation, Luminance).GetHashCode();

        /// <summary>
        /// Converts HSL color to RGB color.
        /// </summary>
        /// <returns>RGB color.</returns>
        public Color ToRgbColor()
        {
            if (Saturation == 0)
            {
                // achromatic color (gray scale}
                return Color.FromArgb(byte.MaxValue,
                    (byte)Math.Round(Luminance * 255.0, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(Luminance * 255.0, MidpointRounding.AwayFromZero),
                    (byte)Math.Round(Luminance * 255.0, MidpointRounding.AwayFromZero)
                );
            }
            else
            {
                {
                    double q = (Luminance < 0.5) ? (Luminance * (1.0 +
                    Saturation)) : (Luminance + Saturation - (Luminance * Saturation));
                    double p = (2.0 * Luminance) - q;
                    double Hk = Hue / 360.0;
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

                    return Color.FromArgb(byte.MaxValue,
                        (byte)Math.Round(T[0] * 255.0, MidpointRounding.AwayFromZero),
                        (byte)Math.Round(T[1] * 255.0, MidpointRounding.AwayFromZero),
                        (byte)Math.Round(T[2] * 255.0, MidpointRounding.AwayFromZero)
                    );
                }
            }
        }
#nullable restore
    }
}
