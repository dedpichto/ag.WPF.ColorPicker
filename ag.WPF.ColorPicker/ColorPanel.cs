using ag.WPF.ColorPicker.ColorHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.ColorPicker
{
    #region Enums
    /// <summary>
    /// Represents available color string representation formats.
    /// </summary>
    public enum ColorStringFormat
    {
        /// <summary>
        /// HEX format.
        /// </summary>
        HEX,
        /// <summary>
        /// ARGB format.
        /// </summary>
        ARGB,
        /// <summary>
        /// RGB format.
        /// </summary>
        RGB,
        /// <summary>
        /// HSB format.
        /// </summary>
        HSB,
        /// <summary>
        /// HSL format.
        /// </summary>
        HSL
    }

    /// <summary>
    /// Represents available <see cref="ColorPanel"/> views.
    /// </summary>
    public enum PanelView
    {
        /// <summary>
        /// Standard view with full controls set available
        /// </summary>
        Standard,
        /// <summary>
        /// Simple minimalistic view
        /// </summary>
        Simple
    }
    #endregion

    /// <summary>
    /// Custom WPF control that allows a user to pick a color from a predefind color palettes.
    /// </summary>
    #region Named parts
    [TemplatePart(Name = "PART_ColorShadingCanvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_ColorShadeSelector", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_SpectrumSlider", Type = typeof(ColorSlider))]
    [TemplatePart(Name = "PART_ColorStringTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_InitialColorPath", Type = typeof(Path))]
    [TemplatePart(Name = "PART_CopyTextBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ShadesPanel", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_TintsPanel", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_Basic", Type = typeof(UniformGrid))]
    [TemplatePart(Name = "PART_TabMain", Type = typeof(TabControl))]
    [TemplatePart(Name = "PART_ListStandard", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_DropPickerBorder", Type = typeof(Border))]
    #endregion

    public class ColorPanel : Control
    {
#nullable disable

        private const string PART_ColorShadingCanvas = "PART_ColorShadingCanvas";
        private const string PART_ColorShadeSelector = "PART_ColorShadeSelector";
        private const string PART_SpectrumSlider = "PART_SpectrumSlider";
        private const string PART_ColorStringTextBox = "PART_ColorStringTextBox";
        private const string PART_InitialColorPath = "PART_InitialColorPath";
        private const string PART_CopyTextBorder = "PART_CopyTextBorder";
        private const string PART_ShadesPanel = "PART_ShadesPanel";
        private const string PART_TintsPanel = "PART_TintsPanel";
        private const string PART_Basic = "PART_Basic";
        private const string PART_TabMain = "PART_TabMain";
        private const string PART_ListStandard = "PART_ListStandard";
        private const string PART_DropPickerBorder = "PART_DropPickerBorder";

        private const int SHADES_COUNT = 12;

        private readonly TranslateTransform _colorShadeSelectorTransform = new();
        private Canvas _colorShadingCanvas;
        private Canvas _colorShadeSelector;
        private ColorSlider _spectrumSlider;
        private TextBox _colorStringTextBox;
        private Path _initialColorPath;
        private Border _copyTextBorder;
        private UniformGrid _shadesPanel;
        private UniformGrid _tintsPanel;
        private UniformGrid _basicPanel;
        private TabControl _tabMain;
        private ListBox _listStandard;
        private Border _dropPickerBorder;

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
        private bool _alphaUpdated;
        private bool _colorTextBoxIsEditing;
        private bool _isSelectedColorUpdating;

        private readonly List<StandardColorItem> _standardColorItems = new();

        #region Dependecy properties
        /// <summary>
        /// The identifier of the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPanel), new FrameworkPropertyMetadata(Colors.Red, OnSelectedColorChanged));
        /// <summary>
        /// The identifier of the <see cref="A"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata(byte.MaxValue, OnAChanged));
        /// <summary>
        /// The identifier of the <see cref="R"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnRGBChanged));
        /// <summary>
        /// The identifier of the <see cref="G"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnRGBChanged));
        /// <summary>
        /// The identifier of the <see cref="B"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorPanel), new FrameworkPropertyMetadata((byte)0, OnRGBChanged));
        /// <summary>
        /// The identifier of the <see cref="HueHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HueHslProperty = DependencyProperty.Register(nameof(HueHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnHueHslChanged));
        /// <summary>
        /// The identifier of the <see cref="SaturationHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaturationHslProperty = DependencyProperty.Register(nameof(SaturationHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnSaturationHslChanged));
        /// <summary>
        /// The identifier of the <see cref="LuminanceHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LuminanceHslProperty = DependencyProperty.Register(nameof(LuminanceHsl), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnLuminanceHslChanged));
        /// <summary>
        /// The identifier of the <see cref="HueHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HueHsbProperty = DependencyProperty.Register(nameof(HueHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnHueHsbChanged));
        /// <summary>
        /// The identifier of the <see cref="SaturationHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaturationHsbProperty = DependencyProperty.Register(nameof(SaturationHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnSaturationHsbChanged));
        /// <summary>
        /// The identifier of the <see cref="BrightnessHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrightnessHsbProperty = DependencyProperty.Register(nameof(BrightnessHsb), typeof(double), typeof(ColorPanel), new FrameworkPropertyMetadata(0.0, OnBrightnessHsbChanged));
        /// <summary>
        /// The identifier of the <see cref="ColorString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorStringProperty = DependencyProperty.Register(nameof(ColorString), typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata(""));
        /// <summary>
        /// The identifier of the <see cref="HorizontalSpectrumBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalSpectrumBrushProperty = DependencyProperty.Register(nameof(HorizontalSpectrumBrush), typeof(LinearGradientBrush), typeof(ColorPanel), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// The identifier of the <see cref="VerticalSpectrumBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalSpectrumBrushProperty = DependencyProperty.Register(nameof(VerticalSpectrumBrush), typeof(LinearGradientBrush), typeof(ColorPanel), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// The identifier of the <see cref="ColorStringFormatProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ColorStringFormatProperty = DependencyProperty.Register(nameof(ColorStringFormat), typeof(ColorStringFormat), typeof(ColorPanel), new FrameworkPropertyMetadata(ColorStringFormat.HEX, OnColorStringFormatChanged));
        #endregion

        #region Attached properties
        /// <summary>
        /// The identifier of the <see cref="TitleTabCustomProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleTabCustomProperty = DependencyProperty.RegisterAttached("TitleTabCustom", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Custom"));
        /// <summary>
        /// The identifier of the <see cref="TitleTabBasicProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleTabBasicProperty = DependencyProperty.RegisterAttached("TitleTabBasic", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Basic"));
        /// <summary>
        /// The identifier of the <see cref="TitleTabStandardProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleTabStandardProperty = DependencyProperty.RegisterAttached("TitleTabStandard", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Standard"));
        /// <summary>
        /// The identifier of the <see cref="TitleFormatProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleFormatProperty = DependencyProperty.RegisterAttached("TitleFormat", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Format"));
        /// <summary>
        /// The identifier of the <see cref="TitleColorModesProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleColorModesProperty = DependencyProperty.RegisterAttached("TitleColorModes", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Color modes"));
        /// <summary>
        /// The identifier of the <see cref="TitleApplyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleApplyProperty = DependencyProperty.RegisterAttached("TitleApply", typeof(string), typeof(ColorPanel), new PropertyMetadata("Apply"));
        /// <summary>
        /// The identifier of the <see cref="TitleCancelProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleCancelProperty = DependencyProperty.RegisterAttached("TitleCancel", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Cancel"));
        /// <summary>
        /// The identifier of the <see cref="TitleShadesAndTintsProperty"/>.
        /// </summary>
        public static readonly DependencyProperty TitleShadesAndTintsProperty = DependencyProperty.RegisterAttached("TitleShadesAndTints", typeof(string), typeof(ColorPanel), new FrameworkPropertyMetadata("Shades and tints"));
        /// <summary>
        /// The identifier of the <see cref="PanelViewProperty"/>.
        /// </summary>
        public static readonly DependencyProperty PanelViewProperty = DependencyProperty.RegisterAttached("PanelView", typeof(PanelView), typeof(ColorPanel), new FrameworkPropertyMetadata(PanelView.Standard));
        #endregion

        #region Routed events
        /// <summary>
        /// The identifier of the <see cref="SelectedColorChanged"/> routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPanel));
        #endregion

        #region Public event handlers
        /// <summary>
        /// Occurrs when <see cref="SelectedColor"/> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, (Delegate)value, false);
            remove => RemoveHandler(SelectedColorChangedEvent, (Delegate)value);
        }
        #endregion

        #region Dependency properties handlers
        /// <summary>
        /// Gets or sets vertical spectrum brush.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public LinearGradientBrush VerticalSpectrumBrush
        {
            get => (LinearGradientBrush)GetValue(VerticalSpectrumBrushProperty);
            private set => SetValue(VerticalSpectrumBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets horizontal spectrum brush.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public LinearGradientBrush HorizontalSpectrumBrush
        {
            get => (LinearGradientBrush)GetValue(HorizontalSpectrumBrushProperty);
            private set => SetValue(HorizontalSpectrumBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets format of selected color's string representation.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ColorStringFormat ColorStringFormat
        {
            get => (ColorStringFormat)GetValue(ColorStringFormatProperty);
            set => SetValue(ColorStringFormatProperty, value);
        }

        /// <summary>
        ///  Gets or sets hue of HSL color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double HueHsl
        {
            get => (double)GetValue(HueHslProperty);
            set => SetValue(HueHslProperty, value);
        }

        /// <summary>
        ///  Gets or sets saturation of HSL color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double SaturationHsl
        {
            get => (double)GetValue(SaturationHslProperty);
            set => SetValue(SaturationHslProperty, value);
        }

        /// <summary>
        ///  Gets or sets luminance of HSL color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double LuminanceHsl
        {
            get => (double)GetValue(LuminanceHslProperty);
            set => SetValue(LuminanceHslProperty, value);
        }

        /// <summary>
        ///  Gets or sets hue of HSB color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double HueHsb
        {
            get => (double)GetValue(HueHsbProperty);
            set => SetValue(HueHsbProperty, value);
        }

        /// <summary>
        ///  Gets or sets saturation of HSB color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double SaturationHsb
        {
            get => (double)GetValue(SaturationHsbProperty);
            set => SetValue(SaturationHsbProperty, value);
        }

        /// <summary>
        ///  Gets or sets brightness of HSB color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double BrightnessHsb
        {
            get => (double)GetValue(BrightnessHsbProperty);
            set => SetValue(BrightnessHsbProperty, value);
        }

        /// <summary>
        /// Gets or sets A value of selected color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte A
        {
            get => (byte)GetValue(AProperty);
            set => SetValue(AProperty, value);
        }

        /// <summary>
        /// Gets or sets R value of selected color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte R
        {
            get => (byte)GetValue(RProperty);
            set => SetValue(RProperty, value);
        }

        /// <summary>
        /// Gets or sets G value of selected color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte G
        {
            get => (byte)GetValue(GProperty);
            set => SetValue(GProperty, value);
        }

        /// <summary>
        /// Gets or sets B value of selected color.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte B
        {
            get => (byte)GetValue(BProperty);
            set => SetValue(BProperty, value);
        }

        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Gets selected color's string representation.
        /// </summary>
        public string ColorString
        {
            get => (string)GetValue(ColorStringProperty);
            private set => SetValue(ColorStringProperty, value);
        }
        #endregion

        #region Attached properties handlers
        /// <summary>
        /// Gets the value of the <see cref="PanelViewProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>One of <see cref="PanelView"/> enum constants.</returns>
        public static PanelView GetPanelView(DependencyObject dependencyObject) => (PanelView)dependencyObject.GetValue(PanelViewProperty);
        /// <summary>
        /// Sets the value of the <see cref="PanelViewProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">One of <see cref="PanelView"/> enum constants.</param>
        public static void SetPanelView(DependencyObject dependencyObject, PanelView value) => dependencyObject.SetValue(PanelViewProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleShadesAndTintsProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleShadesAndTints(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleShadesAndTintsProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleShadesAndTintsProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleShadesAndTints(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleShadesAndTintsProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleCancelProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleCancel(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleCancelProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleCancelProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleCancel(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleCancelProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleApplyProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleApply(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleApplyProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleApplyProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleApply(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleApplyProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleColorModesProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleColorModes(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleColorModesProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleColorModesProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleColorModes(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleColorModesProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleFormatProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleFormat(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleFormatProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleFormatProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleFormat(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleFormatProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleTabStandardProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleTabStandard(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleTabStandardProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleTabStandardProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleTabStandard(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleTabStandardProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleTabBasicProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleTabBasic(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleTabBasicProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleTabBasicProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleTabBasic(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleTabBasicProperty, value);

        /// <summary>
        /// Gets the value of the <see cref="TitleTabCustomProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <returns>String.</returns>
        public static string GetTitleTabCustom(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleTabCustomProperty);
        /// <summary>
        /// Sets the value of the <see cref="TitleTabCustomProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject.</param>
        /// <param name="value">String.</param>
        public static void SetTitleTabCustom(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleTabCustomProperty, value);

        #endregion

        #region OnXXXChanged procedures
        private static void OnHueHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnHueHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnSaturationHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnSaturationHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnLuminanceHslChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnLuminanceHslChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="HueHsl"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnHueHslChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
            _updateHsl = false;
            var hsl = new HslColor(HueHsl, SaturationHsl, LuminanceHsl);
            var color = hsl.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _updateHsl = true;
        }

        /// <summary>
        /// Occurs when the <see cref="SaturationHsl"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnSaturationHslChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
            _updateHsl = false;
            _saturationHslUpdated = true;
            var hsl = new HslColor(HueHsl, SaturationHsl, LuminanceHsl);
            var color = hsl.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _saturationHslUpdated = false;
            _updateHsl = true;
        }

        /// <summary>
        /// Occurs when the <see cref="LuminanceHsl"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnLuminanceHslChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
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
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnHueHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnSaturationHsbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnSaturationHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static void OnBrightnessHsbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnBrightnessHsbChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="HueHsb"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnHueHsbChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
            _updateHsb = false;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _updateHsb = true;
        }

        /// <summary>
        /// Occurs when the <see cref="SaturationHsb"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnSaturationHsbChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
            _updateHsb = false;
            _saturationHsbUpdated = true;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _saturationHsbUpdated = false;
            _updateHsb = true;
        }

        /// <summary>
        /// Occurs when the <see cref="BrightnessHsb"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnBrightnessHsbChanged(double oldValue, double newValue)
        {
            if (_isSelectedColorUpdating)
                return;
            _updateHsb = false;
            _brightnessHsbUpdated = true;
            var hsb = new HsbColor(HueHsb, SaturationHsb, BrightnessHsb);
            var color = hsb.ToRgbColor();
            SelectedColor = Color.FromArgb(A, color.R, color.G, color.B);
            _brightnessHsbUpdated = false;
            _updateHsb = true;
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="SelectedColor"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            _isSelectedColorUpdating = true;
            UpdateRGBValues(newValue);

            if (_updateHsl)
            {
                UpdateHSLValues(newValue);
            }

            if (_updateHsb)
            {
                UpdateHSBValues(newValue);
            }

            ColorString = GetColorString();

            if (!_colorTextBoxIsEditing)
            {
                _colorStringTextBox.Text = ColorString;
            }

            UpdateColorSelectorPosition();

            SelectBasicColor(newValue);

            SelectStandardColor(newValue);

            _isSelectedColorUpdating = false;

            var changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            RaiseEvent(changedEventArgs);
        }

        private static void OnRGBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnRGBChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="R"/>, <see cref="G"/> or <see cref="B"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnRGBChanged(byte oldValue, byte newValue)
        {
            if (_surpressPropertyChanged)
                return;
            UpdateSelectedColor();
        }

        private static void OnAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnAChanged((byte)e.OldValue, (byte)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="A"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnAChanged(byte oldValue, byte newValue)
        {
            if (_surpressPropertyChanged)
                return;
            _alphaUpdated = true;
            UpdateSelectedColor();
            _alphaUpdated = false;
        }

        private static void OnColorStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPanel colorPanel) return;
            colorPanel.OnColorStringFormatChanged((ColorStringFormat)e.OldValue, (ColorStringFormat)e.NewValue);
        }

        /// <summary>
        /// Occurs when the <see cref="ColorStringFormat"/> property has been changed in some way.
        /// </summary>
        protected virtual void OnColorStringFormatChanged(ColorStringFormat oldValue, ColorStringFormat newValue)
        {
            ColorString = GetColorString();
            _colorStringTextBox.Text = ColorString;
        }
        #endregion

        #region ctor
        static ColorPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPanel), new FrameworkPropertyMetadata(typeof(ColorPanel)));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes control for the first time
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            CreateSpectrum();

            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown -= ColorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp -= ColorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove -= ColorShadingCanvas_MouseMove;
                _colorShadingCanvas.SizeChanged -= ColorShadingCanvas_SizeChanged;
            }
            _colorShadingCanvas = GetTemplateChild(PART_ColorShadingCanvas) as Canvas;
            if (_colorShadingCanvas != null)
            {
                _colorShadingCanvas.MouseLeftButtonDown += ColorShadingCanvas_MouseLeftButtonDown;
                _colorShadingCanvas.MouseLeftButtonUp += ColorShadingCanvas_MouseLeftButtonUp;
                _colorShadingCanvas.MouseMove += ColorShadingCanvas_MouseMove;
                _colorShadingCanvas.SizeChanged += ColorShadingCanvas_SizeChanged;
            }

            if (_initialColorPath != null)
            {
                _initialColorPath.MouseLeftButtonDown -= InitialColorPath_MouseLeftButtonDown;
            }
            _initialColorPath = GetTemplateChild(PART_InitialColorPath) as Path;
            if (_initialColorPath != null)
            {
                _initialColorPath.MouseLeftButtonDown += InitialColorPath_MouseLeftButtonDown;
                _initialColorPath.Fill = new SolidColorBrush(SelectedColor);
                _initialColorPath.Stroke = new SolidColorBrush(SelectedColor);
                _initialColor = SelectedColor;
            }

            _colorShadeSelector = GetTemplateChild(PART_ColorShadeSelector) as Canvas;
            if (_colorShadeSelector != null)
                _colorShadeSelector.RenderTransform = _colorShadeSelectorTransform;

            if (_spectrumSlider != null)
            {
                _spectrumSlider.ValueChanged -= SpectrumSlider_ValueChanged;
            }
            _spectrumSlider = GetTemplateChild(PART_SpectrumSlider) as ColorSlider;
            if (_spectrumSlider != null)
            {
                _spectrumSlider.ValueChanged += SpectrumSlider_ValueChanged;
            }

            if (_colorStringTextBox != null)
            {
                _colorStringTextBox.TextChanged -= ColorStringTextBox_TextChanged;
                _colorStringTextBox.PreviewKeyDown -= ColorStringTextBox_PreviewKeyDown;
                _colorStringTextBox.LostFocus += ColorStringTextBox_LostFocus;
            }
            _colorStringTextBox = GetTemplateChild(PART_ColorStringTextBox) as TextBox;
            if (_colorStringTextBox != null)
            {
                _colorStringTextBox.TextChanged += ColorStringTextBox_TextChanged;
                _colorStringTextBox.PreviewKeyDown += ColorStringTextBox_PreviewKeyDown;
                _colorStringTextBox.LostFocus += ColorStringTextBox_LostFocus;
            }

            _tabMain = GetTemplateChild(PART_TabMain) as TabControl;
            _shadesPanel = GetTemplateChild(PART_ShadesPanel) as UniformGrid;
            _tintsPanel = GetTemplateChild(PART_TintsPanel) as UniformGrid;

            if (_basicPanel != null)
            {
                foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
                {
                    radio.Click -= Radio_Click;
                }
            }
            _basicPanel = GetTemplateChild(PART_Basic) as UniformGrid;
            if (_basicPanel != null)
            {
                foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
                {
                    radio.Click += Radio_Click;
                }
            }

            if (_listStandard != null)
            {
                _listStandard.SelectionChanged -= ListStandard_SelectionChanged;
            }
            _listStandard = GetTemplateChild(PART_ListStandard) as ListBox;
            if (_listStandard != null)
            {
                _listStandard.SelectionChanged += ListStandard_SelectionChanged;
                LoadStandardColors();
            }

            if (_dropPickerBorder != null)
            {
                _dropPickerBorder.MouseLeftButtonDown -= DropPickerBorder_MouseLeftButtonDown;
            }
            _dropPickerBorder = GetTemplateChild(PART_DropPickerBorder) as Border;
            if (_dropPickerBorder != null)
            {
                _dropPickerBorder.MouseLeftButtonDown += DropPickerBorder_MouseLeftButtonDown;
            }

            if (_copyTextBorder != null)
            {
                _copyTextBorder.MouseLeftButtonDown -= CopyTextBorder_MouseLeftButtonDown;
            }
            _copyTextBorder = GetTemplateChild(PART_CopyTextBorder) as Border;
            if (_copyTextBorder != null)
            {
                _copyTextBorder.MouseLeftButtonDown += CopyTextBorder_MouseLeftButtonDown;
            }

            UpdateRGBValues(SelectedColor);
            UpdateHSLValues(SelectedColor);
            UpdateHSBValues(SelectedColor);
            ColorString = GetColorString();
            UpdateColorSelectorPosition();

            CreateShadesAndTints();

            if (_colorStringTextBox != null)
                _colorStringTextBox.Text = ColorString;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Returns <see cref="ColorStringFormat"/> enumeration values
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<ColorStringFormat> ColorStringFormats { get; } = new List<ColorStringFormat>(Enum.GetValues(typeof(ColorStringFormat)).Cast<ColorStringFormat>());
        #endregion

        #region Internal properties

        /// <summary>
        /// Gets or sets initial color.
        /// </summary>
        public Color InitialColor
        {
            get => _initialColor;
            set
            {
                _initialColor = value;
                if (_initialColorPath != null)
                {
                    _initialColorPath.Fill = new SolidColorBrush(value);
                    _initialColorPath.Stroke = new SolidColorBrush(value);
                }
                SelectedColor = value;
            }
        }
        #endregion

        #region Private event handlers
        private void ColorStringTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_colorStringTextBox.Text != ColorString)
                _colorStringTextBox.Text = ColorString;
        }

        private void ColorStringTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _colorTextBoxIsEditing = true;
            if (e.Key.In(Key.Back, Key.Delete, Key.Tab, Key.Left, Key.Right, Key.Home, Key.End))
            {
                return;
            }
            switch (ColorStringFormat)
            {
                case ColorStringFormat.HEX:
                    if (!(e.Key >= Key.D0 && e.Key <= Key.D9)
                        && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                        && !(e.Key >= Key.A && e.Key <= Key.F))
                    {
                        e.Handled = true;
                        return;
                    }
                    break;
                case ColorStringFormat.ARGB:
                case ColorStringFormat.RGB:
                    {
                        if (!(e.Key >= Key.D0 && e.Key <= Key.D9)
                            && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                            && e.Key != Key.OemComma)
                        {
                            e.Handled = true;
                            return;
                        }
                        var selectionLength = _colorStringTextBox.SelectionLength;
                        var selectionStart = _colorStringTextBox.SelectionStart;
                        var alteredText = _colorStringTextBox.Text;
                        if (selectionLength == 0)
                        {
                            alteredText = alteredText.Insert(selectionStart, KeyToChar(e.Key));
                        }
                        else
                        {
                            alteredText = alteredText.Substring(0, selectionStart) + KeyToChar(e.Key) + alteredText.Substring(selectionStart + selectionLength);
                        }
                        var arr = alteredText.Split(',');
                        if (ColorStringFormat == ColorStringFormat.ARGB && arr.Length > 4)
                        {
                            e.Handled = true;
                            return;
                        }
                        else if (ColorStringFormat == ColorStringFormat.RGB && arr.Length > 3)
                        {
                            e.Handled = true;
                            return;
                        }
                        if (arr.Any(s => !string.IsNullOrEmpty(s) && Convert.ToInt32(s) > 255))
                        {
                            e.Handled = true;
                            return;
                        }
                        break;
                    }
                case ColorStringFormat.HSB:
                case ColorStringFormat.HSL:
                    {
                        if (!(e.Key >= Key.D0 && e.Key <= Key.D9)
                            && !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                            && !e.Key.In(Key.OemPeriod, Key.Decimal, Key.OemComma))
                        {
                            e.Handled = true;
                            return;
                        }
                        var selectionLength = _colorStringTextBox.SelectionLength;
                        var selectionStart = _colorStringTextBox.SelectionStart;
                        var alteredText = _colorStringTextBox.Text;
                        if (selectionLength == 0)
                        {
                            alteredText = alteredText.Insert(selectionStart, KeyToChar(e.Key));
                        }
                        else
                        {
                            alteredText = alteredText.Substring(0, selectionStart) + KeyToChar(e.Key) + alteredText.Substring(selectionStart + selectionLength);
                        }
                        var arr = alteredText.Split(',');
                        if (arr.Length > 3)
                        {
                            e.Handled = true;
                            return;
                        }
                        if (!string.IsNullOrEmpty(arr[0]))
                        {
                            if (arr[0].Contains("."))
                            {
                                e.Handled = true;
                                return;
                            }
                            if (Convert.ToInt32(arr[0]) > 360)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        foreach (var item in arr.Skip(1))
                        {
                            var split = item.Split('.');
                            if (split.Length > 2)
                            {
                                e.Handled = true;
                                return;
                            }
                            if (split.Length == 2 && split[1].Length > 2)
                            {
                                e.Handled = true;
                                return;
                            }
                            if (!string.IsNullOrEmpty(item) && item != "." && Convert.ToDouble(item) > 1.0)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        break;
                    }
            }
        }

        private void ColorStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_colorStringTextBox.Text == ColorString)
            {
                _colorTextBoxIsEditing = false;
                return;
            }

            switch (ColorStringFormat)
            {
                case ColorStringFormat.HEX:
                    {
                        var temp = _colorStringTextBox.Text.PadRight(8, '0');
                        var arr = new string[4];
                        for (int i = 0, j = 0; i <= 6; i += 2, j++)
                        {
                            arr[j] = temp.Substring(i, 2);
                        }
                        SelectedColor = Color.FromArgb(byte.Parse(arr[0], System.Globalization.NumberStyles.HexNumber),
                            byte.Parse(arr[1], System.Globalization.NumberStyles.HexNumber),
                            byte.Parse(arr[2], System.Globalization.NumberStyles.HexNumber),
                            byte.Parse(arr[3], System.Globalization.NumberStyles.HexNumber));

                        break;
                    }
                case ColorStringFormat.ARGB:
                    {
                        var bytes = new byte[4];
                        var arr = _colorStringTextBox.Text.Split(',');
                        for (var i = 0; i < arr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]))
                                continue;
                            bytes[i] = byte.Parse(arr[i]);
                        }
                        SelectedColor = Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);

                        break;
                    }
                case ColorStringFormat.RGB:
                    {
                        var bytes = new byte[3];
                        var arr = _colorStringTextBox.Text.Split(',');
                        for (var i = 0; i < arr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]))
                                continue;
                            bytes[i] = byte.Parse(arr[i]);
                        }
                        SelectedColor = Color.FromArgb(A, bytes[0], bytes[1], bytes[2]);

                        break;
                    }
                case ColorStringFormat.HSB:
                    {
                        var values = new double[2];
                        var arr = _colorStringTextBox.Text.Split(',');
                        var hueHsb = string.IsNullOrEmpty(arr[0]) ? 0.0 : Convert.ToDouble(arr[0]);
                        for (var i = 1; i < arr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]) || arr[i] == ".")
                                continue;
                            values[i - 1] = Convert.ToDouble(arr[i]);
                        }
                        var saturationHsb = values[0];
                        var brightnessHsb = values[1];
                        var hsb = new HsbColor(hueHsb, saturationHsb, brightnessHsb);
                        SelectedColor = hsb.ToRgbColor();

                        break;
                    }
                case ColorStringFormat.HSL:
                    {
                        var values = new double[2];
                        var arr = _colorStringTextBox.Text.Split(',');
                        var hueHsl = string.IsNullOrEmpty(arr[0]) ? 0.0 : Convert.ToDouble(arr[0]);
                        for (var i = 1; i < arr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i]) || arr[i] == ".")
                                continue;
                            values[i - 1] = Convert.ToDouble(arr[i]);
                        }
                        var saturationHsl = values[0];
                        var luminanceHsl = values[1];
                        var hsl = new HslColor(hueHsl, saturationHsl, luminanceHsl);
                        SelectedColor = hsl.ToRgbColor();

                        break;
                    }
            }
            _colorTextBoxIsEditing = false;
        }

        private void CopyTextBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorStringTextBox.Text != ColorString)
                _colorStringTextBox.Text = ColorString;
            Clipboard.SetText(GetColorString());
        }

        private void DropPickerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorStringTextBox.Text != ColorString)
                _colorStringTextBox.Text = ColorString;
            var pickerPanel = new PickerPanel { Left = 0, Top = 0 };
            var result = pickerPanel.ShowDialog();
            if (result != null && result.Value)
            {
                SelectedColor = pickerPanel.SelectedColor;
            }
        }

        private void ListStandard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            if (e.AddedItems[0] is not StandardColorItem standardColorItem) return;
            var color = standardColorItem.Color;
            if (SelectedColor.R == color.R && SelectedColor.G == color.G && SelectedColor.B == color.B)
                return;
            SelectedColor = Color.FromArgb(SelectedColor.A, color.R, color.G, color.B);
            _tabMain.SelectedIndex = 0;
        }

        private void Radio_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radio) return;
            if (radio.Background is not SolidColorBrush brush) return;
            var color = brush.Color;
            SelectedColor = Color.FromArgb(SelectedColor.A, color.R, color.G, color.B);
            _tabMain.SelectedIndex = 0;
        }

        private void InitialColorPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Path path) return;
            if (path.Fill is not SolidColorBrush brush) return;
            SelectedColor = brush.Color;
        }

        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Rectangle rect) return;
            if (rect.Fill is not SolidColorBrush brush) return;
            SelectedColor = brush.Color;
        }

        private void SpectrumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_currentColorPosition.HasValue)
                return;
            CalculateColor(_currentColorPosition.Value);
        }

        private void ColorShadingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_currentColorPosition.HasValue)
                return;
            UpdateColorSelectorPositionAndCalculateColor(new Point()
            {
                X = _currentColorPosition.Value.X * e.NewSize.Width,
                Y = _currentColorPosition.Value.Y * e.NewSize.Height
            }, false);
        }

        private void ColorShadingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_colorShadingCanvas == null || e.LeftButton != MouseButtonState.Pressed || !_fromMouseMove)
                return;

            UpdateColorSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);

            Mouse.Synchronize();
        }

        private void ColorShadingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;

            _colorShadingCanvas.ReleaseMouseCapture();
            _fromMouseMove = false;
            e.Handled = true;
        }

        private void ColorShadingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_colorShadingCanvas == null)
                return;
            _fromMouseMove = true;
            UpdateColorSelectorPositionAndCalculateColor(e.GetPosition(_colorShadingCanvas), true);
            _colorShadingCanvas.CaptureMouse();
            e.Handled = true;
        }
        #endregion

        #region Private procedures
        private void CreateSpectrum()
        {
            VerticalSpectrumBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0.0),
                EndPoint = new Point(0.5, 1.0),
                ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation
            };
            HorizontalSpectrumBrush = new LinearGradientBrush
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
                VerticalSpectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)index * num));
                HorizontalSpectrumBrush.GradientStops.Add(new GradientStop(hsvSpectrum[index], (double)(hsvSpectrum.Count - index - 1) * num));
            }
            VerticalSpectrumBrush.GradientStops[index - 1].Offset = 1.0;
            HorizontalSpectrumBrush.GradientStops[index - 1].Offset = 0.0;

        }

        private string GetColorString() => ColorStringFormat switch
        {
            ColorStringFormat.HEX => $"{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}",
            ColorStringFormat.ARGB => $"{SelectedColor.A},{SelectedColor.R},{SelectedColor.G},{SelectedColor.B}",
            ColorStringFormat.RGB => $"{SelectedColor.R},{SelectedColor.G},{SelectedColor.B}",
            ColorStringFormat.HSB => $"{HueHsb:f0},{SaturationHsb:f2},{BrightnessHsb:f2}",
            ColorStringFormat.HSL => $"{HueHsl:f0},{SaturationHsl:f2},{LuminanceHsl:f2}",
            _ => $"{SelectedColor.A:X2}{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2}"
        };

        private void SelectBasicColor(Color color)
        {
            if (_basicPanel == null) return;
            foreach (var radio in _basicPanel.Children.OfType<RadioButton>())
            {
                radio.IsChecked = false;
                if (radio.Background is not SolidColorBrush brush)
                    continue;
                if (color.R == brush.Color.R && color.G == brush.Color.G && color.B == brush.Color.B)
                {
                    radio.IsChecked = true;
                }
            }
        }

        private void LoadStandardColors()
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

        private void SelectStandardColor(Color color)
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

        private void CreateShadesAndTints()
        {
            var transparentBackgroundBrush = Utils.TransparentBrush();
            var step = 1.0 / (SHADES_COUNT - 1);
            var factor = 0.0;
            for (var i = 0; i < SHADES_COUNT; i++)
            {
                var border = new Border { Background = transparentBackgroundBrush, MinWidth = 24 };
                var rect = new Rectangle() { Cursor = Cursors.Hand, Stroke = Brushes.Gray };
                rect.SetBinding(Shape.FillProperty, new Binding(nameof(SelectedColor)) { Source = this, Converter = new ColorToBighterOrDarkerConverter(), ConverterParameter = factor });
                rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
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
                var border = new Border { Background = transparentBackgroundBrush, MinWidth = 24 };
                var rect = new Rectangle() { Cursor = Cursors.Hand, Stroke = Brushes.Gray };
                rect.SetBinding(Shape.FillProperty, new Binding(nameof(SelectedColor)) { Source = this, Converter = new ColorToBighterOrDarkerConverter(), ConverterParameter = factor });
                rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
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

        private void UpdateColorSelectorPositionAndCalculateColor(Point p, bool calculateColor)
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

        private void UpdateColorSelectorPosition()
        {
            if (_spectrumSlider == null || _colorShadingCanvas == null)
                return;
            _currentColorPosition = new Point?();

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
            var hsb = new HsbColor(_spectrumSlider.Value, 1.0, 1.0)
            {
                Saturation = p.X,
                Brightness = 1.0 - p.Y
            };
            var rgb = hsb.ToRgbColor();
            rgb.A = A;
            _updateSpectrumSliderValue = false;
            SelectedColor = Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
            // change hue in case of white, black or gray
            if (IsNonColor(SelectedColor))
            {
                HueHsb = hsb.Hue;
            }
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
            if (!_fromMouseMove && !_saturationHslUpdated && !_saturationHsbUpdated && !_brightnessHsbUpdated && !_luminanceHslUpdated && !_alphaUpdated && !IsNonColor(color))
                HueHsl = hsl.Hue;
            SaturationHsl = hsl.Saturation;
            LuminanceHsl = hsl.Luminance;
        }

        private void UpdateHSBValues(Color color)
        {
            var hsb = color.ToHsbColor();
            if (!_fromMouseMove && !_saturationHslUpdated && !_saturationHsbUpdated && !_brightnessHsbUpdated && !_luminanceHslUpdated && !_alphaUpdated && !IsNonColor(color))
                HueHsb = hsb.Hue;
            SaturationHsb = hsb.Saturation;
            BrightnessHsb = hsb.Brightness;
        }

        private static bool IsNonColor(Color color) => (color.R == color.G && color.G == color.B);

        private void UpdateSelectedColor()
        {
            if (!_isSelectedColorUpdating)
                SelectedColor = Color.FromArgb(A, R, G, B);
        }

        private string KeyToChar(Key key)
        {
            return key switch
            {
                Key.A => "a",
                Key.B => "b",
                Key.C => "c",
                Key.D => "d",
                Key.E => "e",
                Key.F => "f",
                Key.D0 or Key.NumPad0 => "0",
                Key.D1 or Key.NumPad1 => "1",
                Key.D2 or Key.NumPad2 => "2",
                Key.D3 or Key.NumPad3 => "3",
                Key.D4 or Key.NumPad4 => "4",
                Key.D5 or Key.NumPad5 => "5",
                Key.D6 or Key.NumPad6 => "6",
                Key.D7 or Key.NumPad7 => "7",
                Key.D8 or Key.NumPad8 => "8",
                Key.D9 or Key.NumPad9 => "9",
                Key.OemComma => ",",
                Key.OemPeriod or Key.Decimal => ".",
                _ => "0"
            };
        }
        #endregion

#nullable restore
    }
}
