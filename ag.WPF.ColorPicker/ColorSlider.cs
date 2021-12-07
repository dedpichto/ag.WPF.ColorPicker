using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ag.WPF.ColorPicker
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ag.WPF.ColorPicker"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ag.WPF.ColorPicker;assembly=ag.WPF.ColorPicker"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    /// 

    [TemplatePart(Name = "PART_SpectrumDisplay", Type = typeof(Rectangle))]

    public class ColorSlider : Slider
    {
        private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";

        private Rectangle _spectrumDisplay;
        private LinearGradientBrush _pickerBrush;

        public static readonly DependencyProperty SelectedColorProperty=DependencyProperty.Register(nameof(SelectedColor),typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Transparent));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _spectrumDisplay = (Rectangle)GetTemplateChild(PART_SpectrumDisplay);
            CreateSpectrum();
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SelectedColor = Utils.ConvertHsvToRgb(360.0 - newValue, 1.0, 1.0);
        }

        private void CreateSpectrum()
        {
            _pickerBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            List<Color> hsvSpectrum = Utils.GenerateHsvPalette();
            double num = 1.0 / (double)(hsvSpectrum.Count - 1);
            int index;
            for (index = 0; index < hsvSpectrum.Count; ++index)
                _pickerBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)index * num));
            _pickerBrush.GradientStops[index - 1].Offset = 1.0;
            if (_spectrumDisplay == null)
                return;
            _spectrumDisplay.Fill = _pickerBrush;
        }
    }
}
