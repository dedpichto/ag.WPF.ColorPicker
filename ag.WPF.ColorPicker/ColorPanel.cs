using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    [TemplatePart(Name = "PART_ColorShadingCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorShadeSelector", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_SpectrumSlider", Type = typeof(ColorSlider))]
    [TemplatePart(Name = "PART_HexadecimalTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_InitialColorRectangle", Type = typeof(Rectangle))]
    [TemplatePart(Name = "PART_CopyHexButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CopyRGBButton", Type = typeof(Button))]

    public class ColorPanel : Control
    {
        private const string PART_ColorShadingCanvas = "PART_ColorShadingCanvas";
        private const string PART_ColorShadeSelector = "PART_ColorShadeSelector";
        private const string PART_SpectrumSlider = "PART_SpectrumSlider";
        private const string PART_HexadecimalTextBox = "PART_HexadecimalTextBox";
        private const string PART_InitialColorRectangle = "PART_InitialColorRectangle";
        private const string PART_CopyHexButton = "PART_CopyHexButton";
        private const string PART_CopyRGBButton = "PART_CopyRGBButton";
        private TranslateTransform _colorShadeSelectorTransform = new TranslateTransform();
        private Canvas _colorShadingCanvas;
        private Canvas _colorShadeSelector;
        private ColorSlider _spectrumSlider;
        private TextBox _hexadecimalTextBox;
        private Rectangle _initialColorRectangle;
        private Button _copyHexButton;
        private Button _copyRGBButton;
        private Point? _currentColorPosition;
        private bool _surpressPropertyChanged;
        private bool _updateSpectrumSliderValue = true;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPanel), new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));
        public static readonly DependencyProperty InitialColorProperty = DependencyProperty.Register(nameof(InitialColor), typeof(Color), typeof(ColorPanel), new FrameworkPropertyMetadata(Colors.Red, OnInitialColorChanged));
        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty HexStringProperty = DependencyProperty.Register(nameof(HexString), typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty RGBStringProperty = DependencyProperty.Register(nameof(RGBString), typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty HexadecimalStringProperty = DependencyProperty.Register(nameof(HexadecimalString), typeof(string), typeof(ColorPanel), new UIPropertyMetadata("", new PropertyChangedCallback(OnHexadecimalStringChanged), new CoerceValueCallback(OnCoerceHexadecimalString)));


        public static readonly DependencyProperty UseAlphaChannelProperty = DependencyProperty.Register(nameof(UseAlphaChannel), typeof(bool), typeof(ColorPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnUseAlphaChannelPropertyChanged)));


        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPanel));

        public string HexString
        {
            get { return (string)GetValue(HexStringProperty);} 
            private set { SetValue(HexStringProperty, value);}
        }

        public string RGBString
        {
            get { return (string)GetValue(RGBStringProperty);}
            private set { SetValue(RGBStringProperty, value);}
        }

        public string HexadecimalString
        {
            get { return (string)GetValue(HexadecimalStringProperty); }
            set { SetValue(HexadecimalStringProperty, value); }
        }

        public bool UseAlphaChannel
        {
            get { return (bool)GetValue(UseAlphaChannelProperty); }
            set { SetValue(UseAlphaChannelProperty, value); }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public Color InitialColor
        {
            get { return (Color)GetValue(InitialColorProperty); }
            set { SetValue(InitialColorProperty, value); }
        }

        public byte A
        {
            get { return (byte)GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        public byte R
        {
            get { return (byte)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }

        public byte G
        {
            get { return (byte)GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }

        public byte B
        {
            get { return (byte)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }

        private static void OnUseAlphaChannelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel))
                return;
            colorPanel.OnUseAlphaChannelChanged();
        }

        protected virtual void OnUseAlphaChannelChanged()
        {
            UpdateSelectedColor();
            SetHexadecimalStringProperty(GetFormatedColorString(SelectedColor), false);
        }

        private static object OnCoerceHexadecimalString(DependencyObject d, object baseValue)
        {
            ColorPanel colorPanel = (ColorPanel)d;
            return colorPanel == null ? baseValue : colorPanel.OnCoerceHexadecimalString(baseValue);
        }

        private object OnCoerceHexadecimalString(object newValue)
        {
            string s = newValue as string;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (int.TryParse(s, NumberStyles.HexNumber, null, out int _))
                        s = "#" + s;
                    ColorConverter.ConvertFromString(s);
                }
            }
            catch
            {
                throw new InvalidDataException("Color provided is not in the correct format.");
            }
            return s;
        }

        private static void OnHexadecimalStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel))
                return;
            colorPanel.OnHexadecimalStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
        {
            string formatedColorString = GetFormatedColorString(newValue);
            if (!GetFormatedColorString(SelectedColor).Equals(formatedColorString))
            {
                Color color = new Color();
                if (!string.IsNullOrEmpty(formatedColorString))
                    color = (Color)ColorConverter.ConvertFromString(formatedColorString);
                UpdateSelectedColor(color);
            }
            SetHexadecimalTextBoxTextProperty(newValue);
        }

        private static void OnInitialColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnInitialColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnInitialColorChanged(Color oldValue, Color newValue)
        {
            SelectedColor = newValue;
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            SetHexadecimalStringProperty(GetFormatedColorString(newValue), false);
            UpdateRGBValues(newValue);
            UpdateColorStrings(newValue);
            UpdateColorShadeSelectorPosition(newValue);
            RoutedPropertyChangedEventArgs<Color> changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            RaiseEvent(changedEventArgs);
        }

        private static void OnByteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnByteChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        protected virtual void OnByteChanged(byte oldValue, byte newValue)
        {
            if (_surpressPropertyChanged)
                return;
            UpdateSelectedColor();
        }

        static ColorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPanel), new FrameworkPropertyMetadata(typeof(ColorPanel)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown -= _colorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp -= _colorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove -= _colorShadingCanvas_MouseMove;
                _colorShadingCanvas.SizeChanged -= _colorShadingCanvas_SizeChanged;
            }
            _colorShadingCanvas = GetTemplateChild(PART_ColorShadingCanvas) as Canvas;
            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown += _colorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp += _colorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove += _colorShadingCanvas_MouseMove;
                _colorShadingCanvas.SizeChanged += _colorShadingCanvas_SizeChanged;
            }

            if (_initialColorRectangle != null)
            {
                _initialColorRectangle.MouseLeftButtonDown -= _initialColorRectangle_MouseLeftButtonDown;
            }
            _initialColorRectangle =GetTemplateChild(PART_InitialColorRectangle) as Rectangle;
            if (_initialColorRectangle != null)
            {
                _initialColorRectangle.MouseLeftButtonDown += _initialColorRectangle_MouseLeftButtonDown;
            }

            if (_copyHexButton != null)
            {
                _copyHexButton.Click -= _copyHexButton_Click;
            }
            _copyHexButton =GetTemplateChild(PART_CopyHexButton) as Button;
            if (_copyHexButton != null)
            {
                _copyHexButton.Click += _copyHexButton_Click;
            }

            if (_copyRGBButton != null)
            {
                _copyRGBButton.Click -= _copyRGBButton_Click;
            }
            _copyRGBButton =GetTemplateChild(PART_CopyRGBButton) as Button;
            if( _copyRGBButton != null)
            {
                _copyRGBButton.Click += _copyRGBButton_Click;
            }

            _colorShadeSelector = GetTemplateChild(PART_ColorShadeSelector) as Canvas;
            if (_colorShadeSelector != null)
                _colorShadeSelector.RenderTransform = _colorShadeSelectorTransform;

            if (_spectrumSlider != null)
            {
                _spectrumSlider.ValueChanged -= _spectrumSlider_ValueChanged;
            }
            _spectrumSlider = GetTemplateChild(PART_SpectrumSlider) as ColorSlider;
            if (_spectrumSlider != null)
            {
                _spectrumSlider.ValueChanged += _spectrumSlider_ValueChanged;
            }

            if (_hexadecimalTextBox != null)
                _hexadecimalTextBox.LostFocus -= _hexadecimalTextBox_LostFocus;
            _hexadecimalTextBox = GetTemplateChild(PART_HexadecimalTextBox) as TextBox;
            if (_hexadecimalTextBox != null)
                _hexadecimalTextBox.LostFocus += _hexadecimalTextBox_LostFocus;

            InitialColor = SelectedColor;

            UpdateRGBValues(SelectedColor);
            UpdateColorStrings(SelectedColor);
            UpdateColorShadeSelectorPosition(SelectedColor);
            SetHexadecimalTextBoxTextProperty(GetFormatedColorString(SelectedColor));

            //var hsb=SelectedColor.ToHsbColor();
            //var hsl=SelectedColor.ToHslColor();

            //var clr1=hsb.ToRgbColor();
            //var clr2=hsl.ToRgbColor();
        }

        private void _copyRGBButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(RGBString);
        }

        private void _copyHexButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(HexString);
        }

        private void _initialColorRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedColor = InitialColor;
        }

        private void SetHexadecimalTextBoxTextProperty(string newValue)
        {
            if (_hexadecimalTextBox == null)
                return;
            _hexadecimalTextBox.Text = newValue;
        }

        private void SetHexadecimalStringProperty(string newValue, bool modifyFromUI)
        {
            if (modifyFromUI)
            {
                try
                {
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        if (int.TryParse(newValue, NumberStyles.HexNumber, null, out int _))
                            newValue = "#" + newValue;
                        ColorConverter.ConvertFromString(newValue);
                    }
                    HexadecimalString = newValue;
                }
                catch
                {
                    SetHexadecimalTextBoxTextProperty(HexadecimalString);
                }
            }
            else
                HexadecimalString = newValue;
        }

        private string GetFormatedColorString(Color colorToFormat) => Utils.FormatColorString(colorToFormat.ToString(), UseAlphaChannel);

        private string GetFormatedColorString(string stringToFormat) => Utils.FormatColorString(stringToFormat, UseAlphaChannel);

        private void UpdateColorShadeSelectorPositionAndCalculateColor(Point p, bool calculateColor)
        {
            if (_colorShadingCanvas == null || _colorShadeSelector == null)
                return;
            if (p.Y < 0.0)
                p.Y = 0.0;
            if (p.X < 0.0)
                p.X = 0.0;
            if (p.X > _colorShadingCanvas.ActualWidth)
                p.X = _colorShadingCanvas.ActualWidth;
            if (p.Y > _colorShadingCanvas.ActualHeight)
                p.Y = _colorShadingCanvas.ActualHeight;
            _colorShadeSelectorTransform.X = p.X - _colorShadeSelector.Width / 2.0;
            _colorShadeSelectorTransform.Y = p.Y - _colorShadeSelector.Height / 2.0;
            p.X /= _colorShadingCanvas.ActualWidth;
            p.Y /= _colorShadingCanvas.ActualHeight;
            _currentColorPosition = new Point?(p);
            if (!calculateColor)
                return;
            CalculateColor(p);
        }

        private void UpdateColorShadeSelectorPosition(Color color)
        {
            if (_spectrumSlider == null || _colorShadingCanvas == null)
                return;
            _currentColorPosition = new Point?();
            var hsv = Utils.ConvertRgbToHsv(color.R, color.G, color.B);
            if (_updateSpectrumSliderValue)
            {
                _spectrumSlider.Value = 360.0 - hsv.H;
                _spectrumSlider.SetAlphaChannel(color.A);
            }

            var point = new Point(hsv.S, 1.0 - hsv.V);
            _currentColorPosition = new Point?(point);
            _colorShadeSelectorTransform.X = point.X * _colorShadingCanvas.Width - 5.0;
            _colorShadeSelectorTransform.Y = point.Y * _colorShadingCanvas.Height - 5.0;
        }

        private void CalculateColor(Point p)
        {
            if (_spectrumSlider == null)
                return;
            var hsvColor = new HsvColor(360.0 - _spectrumSlider.Value, 1.0, 1.0)
            {
                S = p.X,
                V = 1.0 - p.Y
            };
            var rgb = Utils.ConvertHsvToRgb(hsvColor.H, hsvColor.S, hsvColor.V);
            rgb.A = A;
            _updateSpectrumSliderValue = false;
            SelectedColor = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            _updateSpectrumSliderValue = true;
            SetHexadecimalStringProperty(GetFormatedColorString(SelectedColor), false);
        }

        private void UpdateRGBValues(Color color)
        {
            _surpressPropertyChanged = true;
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
            _surpressPropertyChanged = false;
        }

        private void UpdateColorStrings(Color color)
        {
            HexString = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            RGBString = $"rgb({color.R},{color.G},{color.B})";
        }

        private void _hexadecimalTextBox_LostFocus(object sender, RoutedEventArgs e) => SetHexadecimalStringProperty((sender as TextBox).Text, true);

        private void _spectrumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_currentColorPosition.HasValue)
                return;
            CalculateColor(_currentColorPosition.Value);
        }

        private void _colorShadingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_currentColorPosition.HasValue)
                return;
            UpdateColorShadeSelectorPositionAndCalculateColor(new Point()
            {
                X = _currentColorPosition.Value.X * e.NewSize.Width,
                Y = _currentColorPosition.Value.Y * e.NewSize.Height
            }, false);
        }

        private void _colorShadingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_colorShadingCanvas == null || e.LeftButton != MouseButtonState.Pressed)
                return;
            UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);
            Mouse.Synchronize();
        }

        private void _colorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;
            _colorShadingCanvas.ReleaseMouseCapture();
        }

        private void _colorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;
            UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);
            _colorShadingCanvas.CaptureMouse();
            e.Handled = true;
        }

        private void UpdateSelectedColor() => SelectedColor = Color.FromArgb(UseAlphaChannel ? A : (byte)255, R, G, B);

        private void UpdateSelectedColor(Color color)
        {
            SelectedColor = Color.FromArgb(UseAlphaChannel ? color.A : (byte)255, color.R, color.G, color.B);
        }
    }
}
