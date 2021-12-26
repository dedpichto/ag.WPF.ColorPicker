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
                _button.PreviewKeyDown -= _button_PreviewKeyDown;
            }
            _button = GetTemplateChild(PART_Button) as Button;
            if( _button != null)
            {
                _button.Click += _button_Click;
                _button.PreviewKeyDown += _button_PreviewKeyDown;
            }

            _popup = GetTemplateChild(PART_Popup) as Popup;

            if (_colorPanel != null)
            {
                _colorPanel.ColorApplied -= _colorPanel_ColorApplied;
                _colorPanel.ColorCanceled -= _colorPanel_ColorCanceled;
            }
            _colorPanel = GetTemplateChild(PART_ColorPanel) as ColorPanel;
            if(_colorPanel != null)
            {
                _colorPanel.ColorApplied += _colorPanel_ColorApplied;
                _colorPanel.ColorCanceled += _colorPanel_ColorCanceled;
            }
        }

        private void _button_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _popup.IsOpen = false;
            }
        }

        private void _colorPanel_ColorCanceled(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = false;
        }

        private void _colorPanel_ColorApplied(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColor = e.NewValue;
            _popup.IsOpen = false;
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            if (_popup.IsOpen)
                return;
            _popup.IsOpen = true;
        }
    }
}
