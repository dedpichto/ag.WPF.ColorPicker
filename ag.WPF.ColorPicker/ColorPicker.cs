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

    public class ColorPicker : Control
    {
        private const string PART_Button = "PART_Button";
        private const string PART_Popup = "PART_Popup";

        private Button _button;
        private Popup _popup;
        private bool _allowPopup = true;

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Red, OnSelectedColorChanged));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ColorPicker colorPicker)) return;
            colorPicker.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldValue, Color newValue)
        {

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
                _button.Click += _button_Click;
            }
            _button = GetTemplateChild(PART_Button) as Button;
            if( _button != null)
            {
                _button.Click += _button_Click;
            }

            _popup = GetTemplateChild(PART_Popup) as Popup;
            if(_popup != null)
            {
                _popup.Closed += _popup_Closed;
            }
        }

        private void _popup_Closed(object sender, EventArgs e)
        {
            if (Mouse.Captured is Button button && button.Name == PART_Button)
            {
                _allowPopup = false;
            }
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            if (!_allowPopup)
            {
                _allowPopup = true;
                return;
            }
            _popup.IsOpen = true;
        }
    }
}
