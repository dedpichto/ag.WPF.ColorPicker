using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ag.WPF.ColorPicker
{
    /// <summary>
    /// Represents custom control that allows users to pick color
    /// </summary>
    
    #region Named parts
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ColorPanel", Type = typeof(ColorPanel))]
    [TemplatePart(Name = "PART_ColorBorder", Type = typeof(Border))]
    #endregion

    public class ColorPicker : Control
    {
#nullable disable
        private const string PART_Button = "PART_Button";
        private const string PART_Popup = "PART_Popup";
        private const string PART_ColorPanel = "PART_ColorPanel";
        private const string PART_ColorBorder = "PART_ColorBorder";

        private Button _button;
        private Popup _popup;
        private ColorPanel _colorPanel;
        private Border _border;

        #region Dependecy properties
        /// <summary>
        /// The identifier of the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Blue));
        /// <summary>
        /// The identifier of the <see cref="ColorString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorStringProperty = DependencyProperty.Register(nameof(ColorString), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata(""));
        /// <summary>
        /// The identifier of the <see cref="UseAlphaChannel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseAlphaChannelProperty = DependencyProperty.Register(nameof(UseAlphaChannel), typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// The identifier of the <see cref="ColorStringFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorStringFormatProperty = DependencyProperty.Register(nameof(ColorStringFormat), typeof(ColorStringFormat), typeof(ColorPicker), new FrameworkPropertyMetadata(ColorStringFormat.HEX));
        /// <summary>
        /// The identifier of the <see cref="TitleTabCustom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleTabCustomProperty = DependencyProperty.Register(nameof(TitleTabCustom), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Custom"));
        /// <summary>
        /// The identifier of the <see cref="TitleTabBasic"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleTabBasicProperty = DependencyProperty.Register(nameof(TitleTabBasic), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Basic"));
        /// <summary>
        /// The identifier of the <see cref="TitleTabStandard"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleTabStandardProperty = DependencyProperty.Register(nameof(TitleTabStandard), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Standard"));
        /// <summary>
        /// The identifier of the <see cref="TitleFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFormatProperty = DependencyProperty.Register(nameof(TitleFormat), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Format"));
        /// <summary>
        /// The identifier of the <see cref="TitleColorModes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleColorModesProperty = DependencyProperty.Register(nameof(TitleColorModes), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Color modes"));
        /// <summary>
        /// The identifier of the <see cref="TitleUseAlpha"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleUseAlphaProperty = DependencyProperty.Register(nameof(TitleUseAlpha), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Use alpha chanell"));
        /// <summary>
        /// The identifier of the <see cref="TitleApply"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleApplyProperty = DependencyProperty.Register(nameof(TitleApply), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Apply"));
        /// <summary>
        /// The identifier of the <see cref="TitleCancel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleCancelProperty = DependencyProperty.Register(nameof(TitleCancel), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Cancel"));
        /// <summary>
        /// The identifier of the <see cref="TitleShadesAndTints"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleShadesAndTintsProperty = DependencyProperty.Register(nameof(TitleShadesAndTints), typeof(string), typeof(ColorPicker), new FrameworkPropertyMetadata("Shades and tints"));
        #endregion

        /// <summary>
        /// The identifier of the <see cref="SelectedColorChanged"/> routed event.
        /// </summary>
        #region Routed events
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));
        #endregion

        #region Public event handlers
        /// <summary>
        /// Handles the <see cref="SelectedColorChanged"/> routed event.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, (Delegate)value, false);
            remove => RemoveHandler(SelectedColorChangedEvent, (Delegate)value);
        }
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

        /// <summary>
        /// Gets or sets format of selected color's string representation.
        /// </summary>
        public ColorStringFormat ColorStringFormat
        {
            get { return (ColorStringFormat)GetValue(ColorStringFormatProperty); }
            set { SetValue(ColorStringFormatProperty, value); }
        }

        /// <summary>
        /// Gets selected color's string representation.
        /// </summary>
        public string ColorString
        {
            get { return (string)GetValue(ColorStringProperty); }
            private set { SetValue(ColorStringProperty, value); }
        }

        /// <summary>
        /// Specifies whether alpha channel is used.
        /// </summary>
        public bool UseAlphaChannel
        {
            get { return (bool)GetValue(UseAlphaChannelProperty); }
            set { SetValue(UseAlphaChannelProperty, value); }
        }
        /// <summary>
        /// Gets or sets the title of shades and tints group.
        /// </summary>
        public string TitleShadesAndTints
        {
            get => (string)GetValue(TitleShadesAndTintsProperty);
            set { SetValue(TitleShadesAndTintsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of Cancel button.
        /// </summary>
        public string TitleCancel
        {
            get => (string)GetValue(TitleCancelProperty);
            set => SetValue(TitleCancelProperty, value);
        }

        /// <summary>
        /// Gets or sets the title of Apply button.
        /// </summary>
        public string TitleApply
        {
            get => (string)GetValue(TitleApplyProperty);
            set => SetValue(TitleApplyProperty, value);
        }

        /// <summary>
        /// Gets or sets the title of Use lpha channel check box.
        /// </summary>
        public string TitleUseAlpha
        {
            get { return (string)GetValue(TitleUseAlphaProperty); }
            set { SetValue(TitleUseAlphaProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of color modes group.
        /// </summary>
        public string TitleColorModes
        {
            get { return (string)GetValue(TitleColorModesProperty); }
            set { SetValue(TitleColorModesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of Format combo box.
        /// </summary>
        public string TitleFormat
        {
            get => (string)GetValue(TitleFormatProperty);
            set => SetValue(TitleFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the title of Standard colors tab.
        /// </summary>
        public string TitleTabStandard
        {
            get => (string)GetValue(TitleTabStandardProperty);
            set => SetValue(TitleTabStandardProperty, value);
        }

        /// <summary>
        /// Gets or sets the title of Basic colors tab.
        /// </summary>
        public string TitleTabBasic
        {
            get { return (string)GetValue(TitleTabBasicProperty); }
            set { SetValue(TitleTabBasicProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of Custom colors tab.
        /// </summary>
        public string TitleTabCustom
        {
            get { return (string)GetValue(TitleTabCustomProperty); }
            set { SetValue(TitleTabCustomProperty, value); }
        }

        #endregion

        #region ctor
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes control for the first time
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_button != null)
            {
                _button.Click += Button_Click;
                _button.PreviewKeyDown -= Button_PreviewKeyDown;
            }
            _button = GetTemplateChild(PART_Button) as Button;
            if (_button != null)
            {
                _button.Click += Button_Click;
                _button.PreviewKeyDown += Button_PreviewKeyDown;
            }

            if (_popup != null)
            {
                _popup.Opened -= Popup_Opened;
            }
            _popup = GetTemplateChild(PART_Popup) as Popup;
            if( _popup != null)
            {
                _popup.Opened += Popup_Opened;
            }

            if (_colorPanel != null)
            {
                _colorPanel.ColorApplied -= ColorPanel_ColorApplied;
                _colorPanel.ColorCanceled -= ColorPanel_ColorCanceled;
                _colorPanel.PreviewKeyDown -= ColorPanel_PreviewKeyDown;
                
            }
            _colorPanel = GetTemplateChild(PART_ColorPanel) as ColorPanel;
            if (_colorPanel != null)
            {
                _colorPanel.ColorApplied += ColorPanel_ColorApplied;
                _colorPanel.ColorCanceled += ColorPanel_ColorCanceled;
                _colorPanel.PreviewKeyDown += ColorPanel_PreviewKeyDown;
                SetBinding(ColorStringProperty,new Binding("ColorString") { Source = _colorPanel });
            }

            if (_border != null)
            {
                _border.MouseLeftButtonDown -= Border_MouseLeftButtonDown;
            }
            _border = GetTemplateChild(PART_ColorBorder) as Border;
            if (_border != null)
            {
                _border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            }
        }
        #endregion

        #region Private event handlers
        private void Popup_Opened(object sender, EventArgs e)
        {
            if (_colorPanel != null)
            {
                _colorPanel.Focus();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenPopup();
        }

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void ColorPanel_ColorCanceled(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
        }

        private void ColorPanel_ColorApplied(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColor = e.NewValue;
            var changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(e.OldValue, e.NewValue)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            RaiseEvent(changedEventArgs);
            _popup.IsOpen = false;
        }

        private void ColorPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenPopup();
        }
        #endregion

        #region Private procedures
        private void OpenPopup()
        {
            if (_popup.IsOpen)
                return;
            _colorPanel.SetInitialColors(SelectedColor);
            _popup.IsOpen = true;
        }
        #endregion
#nullable restore
    }
}
