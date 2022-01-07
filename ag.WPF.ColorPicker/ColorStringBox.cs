using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ag.WPF.ColorPicker
{
    /// <summary>
    /// Represents custom control that allows user to set color parts values.
    /// </summary>
    #region Named parts
    [TemplatePart(Name = "PART_Stack", Type = typeof(StackPanel))]
    #endregion
    public class ColorStringBox : Control
    {
#nullable disable
        private const string PART_Stack = "PART_Stack";

        private StackPanel _stackPanel;

        private readonly List<TextBox> _boxes = new();

        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="A"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AProperty = DependencyProperty.Register(nameof(A), typeof(byte), typeof(ColorStringBox), new FrameworkPropertyMetadata(byte.MaxValue));
        /// <summary>
        /// The identifier of the <see cref="R"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RProperty = DependencyProperty.Register(nameof(R), typeof(byte), typeof(ColorStringBox), new FrameworkPropertyMetadata((byte)0));
        /// <summary>
        /// The identifier of the <see cref="G"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GProperty = DependencyProperty.Register(nameof(G), typeof(byte), typeof(ColorStringBox), new FrameworkPropertyMetadata((byte)0));
        /// <summary>
        /// The identifier of the <see cref="B"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BProperty = DependencyProperty.Register(nameof(B), typeof(byte), typeof(ColorStringBox), new FrameworkPropertyMetadata((byte)0));
        /// <summary>
        /// The identifier of the <see cref="HueHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HueHslProperty = DependencyProperty.Register(nameof(HueHsl), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// The identifier of the <see cref="SaturationHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaturationHslProperty = DependencyProperty.Register(nameof(SaturationHsl), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// The identifier of the <see cref="LuminanceHsl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LuminanceHslProperty = DependencyProperty.Register(nameof(LuminanceHsl), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// The identifier of the <see cref="HueHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HueHsbProperty = DependencyProperty.Register(nameof(HueHsb), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// The identifier of the <see cref="SaturationHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaturationHsbProperty = DependencyProperty.Register(nameof(SaturationHsb), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        /// <summary>
        /// The identifier of the <see cref="BrightnessHsb"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrightnessHsbProperty = DependencyProperty.Register(nameof(BrightnessHsb), typeof(double), typeof(ColorStringBox), new FrameworkPropertyMetadata(0.0));
        #endregion

        #region Dependency property handlers
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
        #endregion

        #region ctor
        static ColorStringBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorStringBox), new FrameworkPropertyMetadata(typeof(ColorStringBox)));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes control for the first time
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_stackPanel != null)
            {
                foreach (var tb in _stackPanel.Children.OfType<TextBox>())
                {
                    tb.PreviewKeyDown -= Tb_PreviewKeyDown;
                    tb.TextChanged -= Tb_TextChanged;
                    tb.GotFocus -= Tb_GotFocus;
                    tb.PreviewMouseRightButtonUp -= Tb_PreviewMouseRightButtonUp;
                    tb.PreviewMouseLeftButtonDown -= Tb_PreviewMouseLeftButtonDown;
                }
            }
            _stackPanel = GetTemplateChild(PART_Stack) as StackPanel;
            if (_stackPanel != null)
            {
                _boxes.Clear();
                foreach (var tb in _stackPanel.Children.OfType<TextBox>())
                {
                    tb.PreviewKeyDown += Tb_PreviewKeyDown;
                    tb.TextChanged += Tb_TextChanged;
                    tb.GotFocus += Tb_GotFocus;
                    tb.PreviewMouseRightButtonUp += Tb_PreviewMouseRightButtonUp;
                    tb.PreviewMouseLeftButtonDown += Tb_PreviewMouseLeftButtonDown;
                    _boxes.Add(tb);
                }
            }
        }

        private void Tb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBox txt) return;
            txt.Focus();
            e.Handled = true;
        }

        private void Tb_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Tb_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox txt) return;
            txt.SelectAll();
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox txt) return;
            if (!string.IsNullOrWhiteSpace(txt.Text))
            {
                switch ((ColorStringFormat)GetValue(ColorPanel.ColorStringFormatProperty))
                {
                    case ColorStringFormat.ARGB:
                        break;
                    case ColorStringFormat.RGB:
                        break;
                    case ColorStringFormat.HSB:
                        break;
                    case ColorStringFormat.HSL:
                        break;
                }
            }
            var index = _boxes.IndexOf(txt);
            if (txt.Text.Length != txt.MaxLength) return;
            switch (index)
            {
                case 0:
                case 1:
                case 2:
                    _boxes[index + 1].Focus();
                    break;
                case 3:
                    _boxes[0].Focus();
                    break;
            }
        }

        private void Tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox txt) return;
            switch (e.Key)
            {
                case Key.Tab:
                    if (txt.CaretIndex == txt.Text.Length || txt.SelectionLength == txt.Text.Length)
                    {
                        var index = _boxes.IndexOf(txt);
                        switch (index)
                        {
                            case 0:
                            case 1:
                            case 2:
                                _boxes[index + 1].Focus();
                                e.Handled = true;
                                break;
                            case 3:
                                break;
                        }
                    }
                    break;
                case Key.Right:
                    if (txt.CaretIndex == txt.Text.Length || txt.SelectionLength == txt.Text.Length)
                    {
                        var index = _boxes.IndexOf(txt);
                        switch (index)
                        {
                            case 0:
                            case 1:
                            case 2:
                                _boxes[index + 1].Focus();
                                break;
                            case 3:
                                _boxes[0].Focus();
                                break;
                        }
                        e.Handled = true;
                    }
                    break;
                case Key.Left:
                    if (txt.CaretIndex == 0 || txt.SelectionLength == txt.Text.Length)
                    {
                        var index = _boxes.IndexOf(txt);
                        switch (index)
                        {
                            case 1:
                            case 2:
                            case 3:
                                _boxes[index - 1].Focus();
                                break;
                            case 0:
                                _boxes[3].Focus();
                                break;
                        }
                        e.Handled = true;
                    }
                    break;
                default:
                    switch ((ColorStringFormat)GetValue(ColorPanel.ColorStringFormatProperty))
                    {
                        case ColorStringFormat.HEX:
                            if (IsKeyForHex(e.Key))
                                return;
                            else
                                break;
                        case ColorStringFormat.ARGB:
                        case ColorStringFormat.RGB:
                            if (IsKeyForArgb(e.Key))
                                return;
                            else
                                break;
                    }
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Private procedures
        private bool IsKeyForHex(Key key)
        {
            return key switch
            {
                Key.D0
                or Key.NumPad0
                or Key.D1
                or Key.NumPad1
                or Key.D2
                or Key.NumPad2
                or Key.D3
                or Key.NumPad3
                or Key.D4
                or Key.NumPad4
                or Key.D5
                or Key.NumPad5
                or Key.D6
                or Key.NumPad6
                or Key.D7
                or Key.NumPad7
                or Key.D8
                or Key.NumPad8
                or Key.D9
                or Key.NumPad9
                or Key.Delete
                or Key.Back
                or Key.A
                or Key.B
                or Key.C
                or Key.D
                or Key.E
                or Key.F => true,
                _ => false,
            };
        }

        private bool IsKeyForArgb(Key key)
        {
            return key switch
            {
                Key.D0
                or Key.NumPad0
                or Key.D1
                or Key.NumPad1
                or Key.D2
                or Key.NumPad2
                or Key.D3
                or Key.NumPad3
                or Key.D4
                or Key.NumPad4
                or Key.D5
                or Key.NumPad5
                or Key.D6
                or Key.NumPad6
                or Key.D7
                or Key.NumPad7
                or Key.D8
                or Key.NumPad8
                or Key.D9
                or Key.NumPad9
                or Key.Delete
                or Key.Back => true,
                _ => false,
            };
        }
        #endregion
    }
#nullable restore
}
