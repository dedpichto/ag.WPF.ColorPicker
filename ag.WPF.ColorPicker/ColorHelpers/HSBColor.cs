using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //public override bool Equals(Object obj)
        //{
        // if (obj == null || GetType() != obj.GetType()) return false;
        // return (this == (HSB)obj);
        //) |
        //public override int GetHashCode ()
        //{
        // return Hue.GetHashCode() *~ Saturation.GetHashCode() ~*
        // Brightness.GetHashCode() ;
        //} |
    }
}