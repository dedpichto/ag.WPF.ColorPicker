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
        /// Gets selected color's string representation.
        /// </summary>
        public string ColorString
        {
            get { return (string)GetValue(ColorStringProperty); }
            private set { SetValue(ColorStringProperty, value); }
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
