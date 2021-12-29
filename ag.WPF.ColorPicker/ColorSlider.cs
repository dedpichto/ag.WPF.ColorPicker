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
    #region Named parts
    [TemplatePart(Name = "PART_SpectrumDisplay", Type = typeof(Rectangle))]
    #endregion

    public class ColorSlider : Slider
    {

        private byte _alpha = byte.MaxValue;

        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Transparent));
        #endregion

        #region Dependency properties handlers
        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        #endregion

        #region ctor
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }
        #endregion

        #region Overides
        /// <summary>
        /// Initializes control for the first time
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            createSpectrum();
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
            SelectedColor = Color.FromArgb(_alpha, color.R, color.G, color.B);
        }
        #endregion

        #region Internal procedures
        internal void SetAlphaChannel(byte alpha)
        {
            _alpha = alpha;
            SelectedColor = Color.FromArgb(alpha, SelectedColor.R, SelectedColor.G, SelectedColor.B);
        }
        #endregion

        #region Private procedures
        private void createSpectrum()
        {
            var spectrumBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            List<Color> hsvSpectrum = Utils.GenerateHsvPalette();
            var num = 1.0 / (hsvSpectrum.Count - 1);
            int index;
            for (index = 0; index < hsvSpectrum.Count; ++index)
            {
                spectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)index * num));
            }
            spectrumBrush.GradientStops[index - 1].Offset = 1.0;

            Background = spectrumBrush;
        }
        #endregion
    }
}
