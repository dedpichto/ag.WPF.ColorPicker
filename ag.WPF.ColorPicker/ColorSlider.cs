using ag.WPF.ColorPicker.ColorHelpers;
using System.Collections.Generic;
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
        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Gray));
        #endregion

        #region Dependency properties handlers
        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
        #endregion

        #region ctor
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }
        #endregion

        #region OnXXXChanged procedures
        /// <summary>
        /// Occurs when the <see cref="Slider.OnValueChanged(double, double)"/> event occurrs.
        /// </summary>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            var hsb = new HsbColor(newValue, 1.0, 1.0);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(byte.MaxValue, color.R, color.G, color.B);
        }
        #endregion
    }
}
