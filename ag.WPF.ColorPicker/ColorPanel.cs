using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ag.WPF.ColorPicker
{
    #region Named parts
    [TemplatePart(Name = "PART_ColorShadingCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorShadeSelector", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_SpectrumSlider", Type = typeof(ColorSlider))]
    [TemplatePart(Name = "PART_HexadecimalTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_InitialColorPath", Type = typeof(System.Windows.Shapes.Path))]
    [TemplatePart(Name = "PART_CopyTextBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ShadesPanel", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_TintsPanel", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_Basic", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_TabMain", Type = typeof(TabControl))]
    [TemplatePart(Name = "PART_ListStandard", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_DropPickerBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ApplyButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Border))]
    #endregion

    public class ColorPanel : Control
    {
        private const string PART_ColorShadingCanvas = "PART_ColorShadingCanvas";
        private const string PART_ColorShadeSelector = "PART_ColorShadeSelector";
        private const string PART_SpectrumSlider = "PART_SpectrumSlider";
        private const string PART_HexadecimalTextBox = "PART_HexadecimalTextBox";
        private const string PART_InitialColorPath = "PART_InitialColorPath";
        private const string PART_CopyTextBorder = "PART_CopyTextBorder";
        private const string PART_ShadesPanel = "PART_ShadesPanel";
        private const string PART_TintsPanel = "PART_TintsPanel";
        private const string PART_Basic = "PART_Basic";
        private const string PART_TabMain = "PART_TabMain";
        private const string PART_ListStandard = "PART_ListStandard";
        private const string PART_DropPickerBorder = "PART_DropPickerBorder";
        private const string PART_ApplyButton = "PART_ApplyButton";
        private const string PART_CancelButton = "PART_CancelButton";

        private const int SHADES_COUNT = 12;

        private TranslateTransform _colorShadeSelectorTransform = new TranslateTransform();
        private Canvas _colorShadingCanvas;
        private Canvas _colorShadeSelector;
        private ColorSlider _spectrumSlider;
        private TextBox _hexadecimalTextBox;
        private System.Windows.Shapes.Path _initialColorPath;
        private Border _copyTextBorder;
        private UniformGrid _shadesPanel;
        private UniformGrid _tintsPanel;
        private UniformGrid _basicPanel;
        private TabControl _tabMain;
        private ListBox _listStandard;
        private Border _dropPickerBorder;
        private Button _applyButton;
        private Button _cancelButton;
        private Point? _currentColorPosition;
        private Color _initialColor;
        private bool _surpressPropertyChanged;
        private bool _updateSpectrumSliderValue = true;
        private bool _updateHsl = true;
        private bool _updateHsb = true;
        private bool _firstTimeLodaded = true;
        private bool _fromMouseMove = false;
        private bool _saturationHsbUpdated;
        private bool _brightnessHsbUpdated;
        private bool _saturationHslUpdated;
        private bool _luminanceHslUpdated;

        private readonly List<StandardColorItem> _standardColorItems = new List<StandardColorItem>();

        #region Dependecy properties
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPanel), new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));
        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnByteChanged));

        public static readonly DependencyProperty HueHslProperty = DependencyProperty.Register(nameof(HueHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnHueHslChanged));
        public static readonly DependencyProperty SaturationHslProperty = DependencyProperty.Register(nameof(SaturationHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnSaturationHslChanged));
        public static readonly DependencyProperty LuminanceHslProperty = DependencyProperty.Register(nameof(LuminanceHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnLuminanceHslChanged));
        public static readonly DependencyProperty HueHsbProperty = DependencyProperty.Register(nameof(HueHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnHueHsbChanged));
        public static readonly DependencyProperty SaturationHsbProperty = DependencyProperty.Register(nameof(SaturationHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnSaturationHsbChanged));
        public static readonly DependencyProperty BrightnessHsbProperty = DependencyProperty.Register(nameof(BrightnessHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnBrightnessHsbChanged));

        public static readonly DependencyProperty HexStringProperty = DependencyProperty.Register(nameof(HexString), typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty RGBStringProperty = DependencyProperty.Register(nameof(RGBString), typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata(""));

        public static readonly DependencyProperty UseAlphaChannelProperty = DependencyProperty.Register(nameof(UseAlphaChannel), typeof(bool), typeof(ColorPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnUseAlphaChannelPropertyChanged)));

        public static readonly DependencyProperty ShowCommandsPanelProperty = DependencyProperty.Register(nameof(ShowCommandsPanel), typeof(bool), typeof(ColorPanel), new FrameworkPropertyMetadata(true));

        #endregion

        #region Routed events
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPanel));
        public static readonly RoutedEvent ColorAppliedEvent = EventManager.RegisterRoutedEvent("ColorApplied", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPanel));
        public static readonly RoutedEvent ColorCanceledEvent = EventManager.RegisterRoutedEvent("ColorCanceled", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ColorPanel));
        #endregion

        #region Pablic event handlers
        public event RoutedPropertyChangedEventHandler<Color> ColorApplied
        {
            add => AddHandler(ColorAppliedEvent, (Delegate)value, false);
            remove => RemoveHandler(ColorAppliedEvent, (Delegate)value);
        }

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, (Delegate)value, false);
            remove => RemoveHandler(SelectedColorChangedEvent, (Delegate)value);
        }

        public event RoutedEventHandler ColorCanceled
        {
            add => AddHandler(ColorCanceledEvent, (Delegate)value, false);
            remove => RemoveHandler(ColorCanceledEvent, (Delegate)value);
        }
        #endregion

        #region Dependency properties handlers
        public bool ShowCommandsPanel
        {
            get { return (bool)GetValue(ShowCommandsPanelProperty); }
            set { SetValue(ShowCommandsPanelProperty, value); }
        }

        public string HexString
        {
            get { return (string)GetValue(HexStringProperty); }
            private set { SetValue(HexStringProperty, value); }
        }

        public string RGBString
        {
            get { return (string)GetValue(RGBStringProperty); }
            private set { SetValue(RGBStringProperty, value); }
        }

        public bool UseAlphaChannel
        {
            get { return (bool)GetValue(UseAlphaChannelProperty); }
            set { SetValue(UseAlphaChannelProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double HueHsl
        {
            get { return (double)GetValue(HueHslProperty); }
            set { SetValue(HueHslProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double SaturationHsl
        {
            get { return (double)GetValue(SaturationHslProperty); }
            set { SetValue(SaturationHslProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double LuminanceHsl
        {
            get { return (double)GetValue(LuminanceHslProperty); }
            set { SetValue(LuminanceHslProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double HueHsb
        {
            get { return (double)GetValue(HueHsbProperty); }
            set { SetValue(HueHsbProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double SaturationHsb
        {
            get { return (double)GetValue(SaturationHsbProperty); }
            set { SetValue(SaturationHsbProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double BrightnessHsb
        {
            get { return (double)GetValue(BrightnessHsbProperty); }
            set { SetValue(BrightnessHsbProperty, value); }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte A
        {
            get { return (byte)GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte R
        {
            get { return (byte)GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte G
        {
            get { return (byte)GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte B
        {
            get { return (byte)GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }
        #endregion

        #region OnXXXChanged procedures
        private static void OnHueHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnHueHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnSaturationHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnSaturationHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnLuminanceHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnLuminanceHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnHueHslChanged(double oldValue, double newValue)
        {
            _updateHsl = false;
            var hsl = new HslColor(HueHsl, SaturationHsl, LuminanceHsl);
            var color = hsl.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _updateHsl = true;
        }

        protected virtual void OnSaturationHslChanged(double oldValue, double newValue)
        {
            _updateHsl = false;
            _saturationHslUpdated = true;
            var hsl = new HslColor(HueHsl, SaturationHsl, LuminanceHsl);
            var color = hsl.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _saturationHslUpdated = false;
            _updateHsl = true;
        }

        protected virtual void OnLuminanceHslChanged(double oldValue, double newValue)
        {
            _updateHsl = false;
            _luminanceHslUpdated = true;
            var hsl = new HslColor(HueHsl, SaturationHsl, LuminanceHsl);
            var color = hsl.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _luminanceHslUpdated = false;
            _updateHsl = true;
        }

        private static void OnHueHsbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnHueHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnSaturationHsbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnSaturationHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnBrightnessHsbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnBrightnessHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnHueHsbChanged(double oldValue, double newValue)
        {
            _updateHsb = false;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _updateHsb = true;
        }

        protected virtual void OnSaturationHsbChanged(double oldValue, double newValue)
        {
            _updateHsb = false;
            _saturationHsbUpdated = true;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _saturationHsbUpdated = false;
            _updateHsb = true;
        }

        protected virtual void OnBrightnessHsbChanged(double oldValue, double newValue)
        {
            _updateHsb = false;
            _brightnessHsbUpdated = true;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _brightnessHsbUpdated = false;
            _updateHsb = true;
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
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPanel colorPanel)) return;
            colorPanel.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            UpdateRGBValues(newValue);

            if (_updateHsl)
            {
                UpdateHSLValues(newValue);
            }

            if (_updateHsb)
            {
                UpdateHSBValues(newValue);
            }

            UpdateColorStrings(newValue);
            UpdateColorShadeSelectorPosition(newValue);

            selectBasicColor(newValue);

            selectStandardColor(newValue);

            if (!ShowCommandsPanel)
            {
                var changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue)
                {
                    RoutedEvent = SelectedColorChangedEvent
                };
                RaiseEvent(changedEventArgs);
            }
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
        #endregion

        #region ctor
        static ColorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPanel), new FrameworkPropertyMetadata(typeof(ColorPanel)));
        } 
        #endregion

        #region Overrides
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

            if (_initialColorPath != null)
            {
                _initialColorPath.MouseLeftButtonDown -= _initialColorPath_MouseLeftButtonDown;
            }
            _initialColorPath = GetTemplateChild(PART_InitialColorPath) as System.Windows.Shapes.Path;
            if (_initialColorPath != null)
            {
                _initialColorPath.MouseLeftButtonDown += _initialColorPath_MouseLeftButtonDown;
                _initialColorPath.Fill = new SolidColorBrush(SelectedColor);
                _initialColorPath.Stroke = new SolidColorBrush(SelectedColor);
                _initialColor = SelectedColor;
            }

            if (_applyButton != null)
            {
                _applyButton.Click -= _applyButton_Click;
            }
            _applyButton = GetTemplateChild(PART_ApplyButton) as Button;
            if (_applyButton != null)
            {
                _applyButton.Click += _applyButton_Click;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Click -= _cancelButton_Click;
            }
            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;
            if (_cancelButton != null)
            {
                _cancelButton.Click += _cancelButton_Click;
            }
            //if (_copyHexButton != null)
            //{
            //    _copyHexButton.Click -= _copyHexButton_Click;
            //}
            //_copyHexButton = GetTemplateChild(PART_CopyTextBorder) as Button;
            //if (_copyHexButton != null)
            //{
            //    _copyHexButton.Click += _copyHexButton_Click;
            //}

            //if (_copyRGBButton != null)
            //{
            //    _copyRGBButton.Click -= _copyRGBButton_Click;
            //}
            //_copyRGBButton = GetTemplateChild(PART_CopyRGBButton) as Button;
            //if (_copyRGBButton != null)
            //{
            //    _copyRGBButton.Click += _copyRGBButton_Click;
            //}

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

            _hexadecimalTextBox = GetTemplateChild(PART_HexadecimalTextBox) as TextBox;

            _tabMain = GetTemplateChild(PART_TabMain) as TabControl;
            _shadesPanel = GetTemplateChild(PART_ShadesPanel) as UniformGrid;
            _tintsPanel = GetTemplateChild(PART_TintsPanel) as UniformGrid;

            if (_basicPanel != null)
            {
                foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
                {
                    radio.Click -= _radio_Click;
                }
            }
            _basicPanel = GetTemplateChild(PART_Basic) as UniformGrid;
            if (_basicPanel != null)
            {
                foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
                {
                    radio.Click += _radio_Click;
                }
            }

            if (_listStandard != null)
            {
                _listStandard.SelectionChanged -= _listStandard_SelectionChanged;
            }
            _listStandard = GetTemplateChild(PART_ListStandard) as ListBox;
            if (_listStandard != null)
            {
                _listStandard.SelectionChanged += _listStandard_SelectionChanged;
                loadStandardColors();
            }

            if (_dropPickerBorder != null)
            {
                _dropPickerBorder.MouseLeftButtonDown -= _dropPickerBorder_MouseLeftButtonDown;
            }
            _dropPickerBorder = GetTemplateChild(PART_DropPickerBorder) as Border;
            if (_dropPickerBorder != null)
            {
                _dropPickerBorder.MouseLeftButtonDown += _dropPickerBorder_MouseLeftButtonDown;
            }

            if (_copyTextBorder != null)
            {
                _copyTextBorder.MouseLeftButtonDown -= _copyTextBorder_MouseLeftButtonDown;
            }
            _copyTextBorder = GetTemplateChild(PART_CopyTextBorder) as Border;
            if (_copyTextBorder != null)
            {
                _copyTextBorder.MouseLeftButtonDown += _copyTextBorder_MouseLeftButtonDown;
            }

            UpdateRGBValues(SelectedColor);
            UpdateHSLValues(SelectedColor);
            UpdateHSBValues(SelectedColor);
            UpdateColorStrings(SelectedColor);
            UpdateColorShadeSelectorPosition(SelectedColor);

            createsShadesAndTints();

        }
        #endregion

        #region Private event handlers
        private void _cancelButton_Click(object sender, RoutedEventArgs e)
        {
            raiseColorCanceledEvent();
        }

        private void _applyButton_Click(object sender, RoutedEventArgs e)
        {
            raiseColorAppliedEvent();
        }

        private void _copyTextBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(_hexadecimalTextBox.Text);
        }

        private void _dropPickerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pickerPanel = new PickerPanel { Left = 0, Top = 0 };
            var result = pickerPanel.ShowDialog();
            if (result != null && result.Value)
            {
                SelectedColor = pickerPanel.SelectedColor;
            }
        }

        private void _listStandard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            if (!(e.AddedItems[0] is StandardColorItem standardColorItem)) return;
            var color = standardColorItem.Color;
            if (SelectedColor.R == color.R && SelectedColor.G == color.G && SelectedColor.B == color.B)
                return;
            SelectedColor = Color.FromArgb(SelectedColor.A, color.R, color.G, color.B);
            _tabMain.SelectedIndex = 0;
        }

        private void _radio_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is RadioButton radio)) return;
            if (!(radio.Background is SolidColorBrush brush)) return;
            var color = brush.Color;
            SelectedColor = Color.FromArgb(SelectedColor.A, color.R, color.G, color.B);
            _tabMain.SelectedIndex = 0;
        }

        private void _initialColorPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is System.Windows.Shapes.Path path)) return;
            if (!(path.Fill is SolidColorBrush brush)) return;
            SelectedColor = brush.Color;
        }

        private void _rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Rectangle rect)) return;
            if (!(rect.Fill is SolidColorBrush brush)) return;
            SelectedColor = brush.Color;
        }

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
            if (_colorShadingCanvas == null || e.LeftButton != MouseButtonState.Pressed || !_fromMouseMove)
                return;

            UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);

            Mouse.Synchronize();
        }

        private void _colorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;

            _colorShadingCanvas.ReleaseMouseCapture();
            _fromMouseMove = false;
            e.Handled = true;
        }

        private void _colorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;
            _fromMouseMove = true;
            UpdateColorShadeSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);
            _colorShadingCanvas.CaptureMouse();
            e.Handled = true;
        }
        #endregion

        #region Private procedures
        private void raiseColorAppliedEvent()
        {
            var appliedEventArgs = new RoutedPropertyChangedEventArgs<Color>(_initialColor, SelectedColor)
            {
                RoutedEvent = ColorAppliedEvent
            };
            RaiseEvent(appliedEventArgs);
        }

        private void raiseColorCanceledEvent()
        {
            var canceledEventArgs = new RoutedEventArgs { RoutedEvent = ColorCanceledEvent };
            RaiseEvent(canceledEventArgs);
        }

        private void selectBasicColor(Color color)
        {
            if (_basicPanel == null) return;
            foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
            {
                radio.IsChecked = false;
                if (!(radio.Background is SolidColorBrush brush))
                    continue;
                if (color.R == brush.Color.R && color.G == brush.Color.G && color.B == brush.Color.B)
                {
                    radio.IsChecked = true;
                }
            }
        }

        private void loadStandardColors()
        {
            if (_listStandard is null) return;
            if (_listStandard.Items.Count > 0)
                return;
            var standardColors = Utils.KnownColors;
            foreach (var color in standardColors.OrderBy(kvp => kvp.Value.Item2.Hue).ThenBy(kvp => kvp.Value.Item2.Saturation).ThenBy(kvp => kvp.Value.Item2.Brightness))
            {
                var colorItem = new StandardColorItem(color.Value.Item1, color.Key);
                _standardColorItems.Add(colorItem);
            }
            _listStandard.ItemsSource = _standardColorItems;
        }

        private void selectStandardColor(Color color)
        {
            if (_listStandard is null) return;
            _listStandard.SelectedIndex = -1;
            foreach (var colorItem in _standardColorItems)
            {
                if (colorItem.Color.R == color.R && colorItem.Color.G == color.G && colorItem.Color.B == color.B)
                {
                    _listStandard.SelectedItem = colorItem;
                    _listStandard.ScrollIntoView(colorItem);
                    break;
                }
            }
        }

        private void createsShadesAndTints()
        {
            var transparentBackgroundBrush = Utils.TransparentBrush();
            var step = 1.0 / (SHADES_COUNT - 1);
            var factor = 0.0;
            for (var i = 0; i < SHADES_COUNT; i++)
            {
                var border = new Border { Background = transparentBackgroundBrush };
                var rect = new Rectangle() { Cursor = Cursors.Hand, Stroke = Brushes.Gray };
                rect.SetBinding(Rectangle.FillProperty, new Binding(nameof(SelectedColor)) { Source = this, Converter = new ColorToBighterOrDarkerConverter(), ConverterParameter = factor });
                rect.MouseLeftButtonDown += _rect_MouseLeftButtonDown;
                border.Child = rect;
                if (i == 0)
                    border.Margin = new Thickness(4, 4, 4, 2);
                else if (i == 11)
                    border.Margin = new Thickness(4, 2, 4, 4);
                else
                    border.Margin = new Thickness(4);
                _tintsPanel.Children.Add(border);

                if (i < SHADES_COUNT - 2)
                    factor += step;
                else
                    factor = 1.0;
            }

            factor = 0.0;
            for (var i = 0; i < SHADES_COUNT; i++)
            {
                var border = new Border { Background = transparentBackgroundBrush };
                var rect = new Rectangle() { Cursor = Cursors.Hand, Stroke = Brushes.Gray };
                rect.SetBinding(Rectangle.FillProperty, new Binding(nameof(SelectedColor)) { Source = this, Converter = new ColorToBighterOrDarkerConverter(), ConverterParameter = factor });
                rect.MouseLeftButtonDown += _rect_MouseLeftButtonDown;
                border.Child = rect;
                if (i == 0)
                    border.Margin = new Thickness(4, 4, 4, 2);
                else if (i == 11)
                    border.Margin = new Thickness(4, 2, 4, 4);
                else
                    border.Margin = new Thickness(4);
                _shadesPanel.Children.Add(border);

                if (i < SHADES_COUNT - 2)
                    factor -= step;
                else
                    factor = -1.0;
            }
        }

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
            //var hsb = color.ToHsbColor();
            //var hueValue = HueHsb;// Math.Round(hsb.Hue, MidpointRounding.AwayFromZero);
            if (_updateSpectrumSliderValue || _fromMouseMove)
            {
                //_spectrumSlider.Value = 360.0 - hsb.Hue;
                if (_firstTimeLodaded)
                {
                    _firstTimeLodaded = false;
                    if (_spectrumSlider.Value != HueHsb)
                    {
                        _spectrumSlider.Value = HueHsb;
                    }
                    else
                    {
                        _spectrumSlider.Value = _spectrumSlider.Value < 360.0 ? _spectrumSlider.Value + 1.0 : _spectrumSlider.Value - 1.0;
                        _spectrumSlider.Value = HueHsb;
                    }
                }
                else
                {
                    _spectrumSlider.Value = HueHsb;
                }
                _spectrumSlider.SetAlphaChannel(color.A);
            }

            var point = new Point(SaturationHsb, 1.0 - BrightnessHsb);
            _currentColorPosition = new Point?(point);
            _colorShadeSelectorTransform.X = point.X * _colorShadingCanvas.Width - 5.0;
            _colorShadeSelectorTransform.Y = point.Y * _colorShadingCanvas.Height - 5.0;
        }

        private void CalculateColor(Point p)
        {
            if (_spectrumSlider == null)
                return;
            //var hsb = new HsbColor(360.0 - _spectrumSlider.Value, 1.0, 1.0)
            var hsb = new HsbColor(_spectrumSlider.Value, 1.0, 1.0)
            {
                Saturation = p.X,
                Brightness = 1.0 - p.Y
            };
            var rgb = hsb.ToRgbColor();
            rgb.A = A;
            _updateSpectrumSliderValue = false;
            SelectedColor = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            _updateSpectrumSliderValue = true;
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

        private void UpdateHSLValues(Color color)
        {
            var hsl = color.ToHslColor();
            if (!_fromMouseMove && !_saturationHslUpdated && !_saturationHsbUpdated && !_brightnessHsbUpdated && !_luminanceHslUpdated && !isRGBGray(color))
                HueHsl = hsl.Hue;
            SaturationHsl = hsl.Saturation;
            LuminanceHsl = hsl.Luminance;
        }

        private void UpdateHSBValues(Color color)
        {
            var hsb = color.ToHsbColor();
            if (!_fromMouseMove && !_saturationHslUpdated && !_saturationHsbUpdated && !_brightnessHsbUpdated && !_luminanceHslUpdated && !isRGBGray(color))
                HueHsb = hsb.Hue;
            SaturationHsb = hsb.Saturation;
            BrightnessHsb = hsb.Brightness;
        }

        private void UpdateColorStrings(Color color)
        {
            HexString = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            RGBString = $"rgb({color.R},{color.G},{color.B})";
        }

        private bool isRGBGray(Color color)
        {
            return (color.R == color.G && color.G == color.B);
        }

        private void UpdateSelectedColor() => SelectedColor = Color.FromArgb(UseAlphaChannel ? A : (byte)255, R, G, B);
        #endregion
    }
}
