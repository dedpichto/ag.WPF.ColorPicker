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
    /// Custom WPF control that allows a user to pick a color from a predefind color palettes and/or screen.
    /// </summary>

    #region Named parts
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ColorContent", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ColorPanel", Type = typeof(ColorPanel))]
    [TemplatePart(Name = "PART_ApplyButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(Border))]
    #endregion

    public class ColorPicker : Control
    {
#nullable disable
        private const string PART_Button = "PART_Button";
        private const string PART_ColorContent = "PART_ColorContent";
        private const string PART_Popup = "PART_Popup";
        private const string PART_ColorPanel = "PART_ColorPanel";
        private const string PART_ApplyButton = "PART_ApplyButton";
        private const string PART_CancelButton = "PART_CancelButton";

        private Button _button, _colorContentButton;
        private Popup _popup;
        private ColorPanel _colorPanel;
        private Button _applyButton;
        private Button _cancelButton;

        private bool _allowPopup = true;

        #region Dependecy properties
        /// <summary>
        /// The identifier of the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Red));
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

            if (_colorContentButton != null)
            {
                _colorContentButton.Click -= ColorContentButton_Click;
                _colorContentButton.PreviewKeyDown -= ColorContentButton_PreviewKeyDown;
            }
            _colorContentButton = GetTemplateChild(PART_ColorContent) as Button;
            if (_colorContentButton != null)
            {
                _colorContentButton.Click += ColorContentButton_Click;
                _colorContentButton.PreviewKeyDown += ColorContentButton_PreviewKeyDown;
            }

            if (_popup != null)
            {
                _popup.Opened -= Popup_Opened;
                _popup.Closed += Popup_Closed;
            }
            _popup = GetTemplateChild(PART_Popup) as Popup;
            if (_popup != null)
            {
                _popup.Opened += Popup_Opened;
                _popup.Closed += Popup_Closed;
            }

            if (_colorPanel != null)
            {
                _colorPanel.PreviewKeyDown -= ColorPanel_PreviewKeyDown;
            }
            _colorPanel = GetTemplateChild(PART_ColorPanel) as ColorPanel;
            if (_colorPanel != null)
            {
                _colorPanel.PreviewKeyDown += ColorPanel_PreviewKeyDown;
                SetBinding(ColorStringProperty, new Binding("ColorString") { Source = _colorPanel });
            }

            if (_applyButton != null)
            {
                _applyButton.Click -= ApplyButton_Click;
            }
            _applyButton = GetTemplateChild(PART_ApplyButton) as Button;
            if (_applyButton != null)
            {
                _applyButton.Click += ApplyButton_Click;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Click -= CancelButton_Click;
            }
            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;
            if (_cancelButton != null)
            {
                _cancelButton.Click += CancelButton_Click;
            }
        }


        #endregion

        #region Private event handlers
        private void CancelButton_Click(object sender, RoutedEventArgs e) => _popup.IsOpen = false;

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedColor= _colorPanel.SelectedColor;
            var changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(_colorPanel.InitialColor, SelectedColor)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            RaiseEvent(changedEventArgs);
            _popup.IsOpen = false;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (Mouse.Captured is Button button && (button.Name == PART_Button || button.Name == PART_ColorContent))
            {
                _allowPopup = false;
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            if (_colorPanel != null)
            {
                _colorPanel.Focus();
            }
        }

        private void ColorPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void Button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => OpenPopup();

        private void ColorContentButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void ColorContentButton_Click(object sender, RoutedEventArgs e) => OpenPopup();
        #endregion

        #region Private procedures
        private void OpenPopup()
        {
            if (!_allowPopup)
            {
                _allowPopup = true;
                return;
            }
            _colorPanel.InitialColor = SelectedColor;
            _popup.IsOpen = true;
        }
        #endregion
#nullable restore
    }
}
