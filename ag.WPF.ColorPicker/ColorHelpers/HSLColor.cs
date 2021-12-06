using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    public struct HslColor
    {
        /// <summary>
        /// Gets an empty HSL structure;
        /// </summary>
        public static readonly HslColor Empty = new HslColor();

        private double hue;
        private double saturation;
        private double luminance;

        public static bool operator ==(HslColor item1, HslColor item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Luminance == item2.Luminance
                );
        }

        public static bool operator !=(HslColor item1, HslColor item2)
        {
            return (
                item1.Hue != item2.Hue
                || item1.Saturation != item2.Saturation
                || item1.Luminance != item2.Luminance
                );
        }

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
            this.hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            this.saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            this.luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HslColor)obj);
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Luminance.GetHashCode();
        }
    }
}
