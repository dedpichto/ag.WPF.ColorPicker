using System;
using System.Collections.Generic;
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
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ColorPanel", Type = typeof(ColorPanel))]

    public class ColorPicker : Control
    {
        private const string PART_Button = "PART_Button";
        private const string PART_Popup = "PART_Popup";
        private const string PART_ColorPanel = "PART_ColorPanel";

        private Button _button;
        private Popup _popup;
        private ColorPanel _colorPanel;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Transparent));

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, (Delegate)value, false);
            remove => RemoveHandler(SelectedColorChangedEvent, (Delegate)value);
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_button != null)
            {
                _button.Click += button_Click;
                _button.PreviewKeyDown -= button_PreviewKeyDown;
            }
            _button = GetTemplateChild(PART_Button) as Button;
            if (_button != null)
            {
                _button.Click += button_Click;
                _button.PreviewKeyDown += button_PreviewKeyDown;
            }

            _popup = GetTemplateChild(PART_Popup) as Popup;

            if (_colorPanel != null)
            {
                _colorPanel.ColorApplied -= colorPanel_ColorApplied;
                _colorPanel.ColorCanceled -= colorPanel_ColorCanceled;
            }
            _colorPanel = GetTemplateChild(PART_ColorPanel) as ColorPanel;
            if (_colorPanel != null)
            {
                _colorPanel.ColorApplied += colorPanel_ColorApplied;
                _colorPanel.ColorCanceled += colorPanel_ColorCanceled;
            }
        }

        private void button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void colorPanel_ColorCanceled(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
        }

        private void colorPanel_ColorApplied(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColor = e.NewValue;
            var changedEventArgs = new RoutedPropertyChangedEventArgs<Color>(e.OldValue, e.NewValue)
            {
                RoutedEvent = SelectedColorChangedEvent
            };
            RaiseEvent(changedEventArgs);
            _popup.IsOpen = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (_popup.IsOpen)
                return;
            _colorPanel.SetInitialColors(SelectedColor, SelectedColor);
            _popup.IsOpen = true;
        }
    }
}
