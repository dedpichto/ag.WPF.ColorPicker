﻿using ag.WPF.ColorPicker.ColorHelpers;
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
    [TemplatePart(Name = "PART_SpectrumDisplay", Type = typeof(Rectangle))]

    public class ColorSlider : Slider
    {
        private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";

        private Rectangle _spectrumDisplay;
        private byte _alpha = byte.MaxValue;
        private LinearGradientBrush _spectrumBrush;
        public static readonly DependencyProperty SpectrumBrushProperty = DependencyProperty.Register(nameof(SpectrumBrush), typeof(LinearGradientBrush), typeof(ColorSlider), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorSlider), new PropertyMetadata(Colors.Transparent));

        public LinearGradientBrush SpectrumBrush
        {
            get { return (LinearGradientBrush)GetValue(SpectrumBrushProperty); }
            set { SetValue(SpectrumBrushProperty, value); }
        }

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
            var hsb = new HsbColor(360.0 - newValue, 1.0, 1.0);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(_alpha, color.R, color.G, color.B);
        }

        internal void SetAlphaChannel(byte alpha)
        {
            _alpha = alpha;
            SelectedColor = Color.FromArgb(alpha, SelectedColor.R, SelectedColor.G, SelectedColor.B);
        }

        private void CreateSpectrum()
        {
            _spectrumBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            SpectrumBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.0, 0.5),
                EndPoint = new Point(1.0, 0.5),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            List<Color> hsvSpectrum = Utils.GenerateHsvPalette();
            var num = 1.0 / (hsvSpectrum.Count - 1);
            int index;
            for (index = 0; index < hsvSpectrum.Count; ++index)
            {
                _spectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)index * num));
                SpectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)(hsvSpectrum.Count-index-1) * num));
            }
            _spectrumBrush.GradientStops[index - 1].Offset = 1.0;
            SpectrumBrush.GradientStops[index - 1].Offset = 0.0;
            //for (index=hsvSpectrum.Count-1; index > 0; --index)
            //{
            //    SpectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)index * num));
            //}
            //SpectrumBrush.GradientStops[0].Offset = 1.0;

            if (_spectrumDisplay == null)
                return;
            _spectrumDisplay.Fill = _spectrumBrush;
            //SpectrumBrush = _spectrumBrush.CloneCurrentValue();
            //SpectrumBrush.StartPoint = new Point(0, 0.5);
            //SpectrumBrush.EndPoint = new Point(1, 0.5);
        }
    }
}
