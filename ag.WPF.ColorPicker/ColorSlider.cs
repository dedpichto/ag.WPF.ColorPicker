using ag.WPF.ColorPicker.ColorHelpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.ColorPicker
{
    /// <summary>
    /// Represents custom slider
    /// </summary>
    public class ColorSlider : Slider
    {
        #region ctor
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }
        #endregion
    }
}
