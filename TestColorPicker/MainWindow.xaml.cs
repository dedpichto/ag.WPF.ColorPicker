using System;
using System.Collections.Generic;
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

namespace TestColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //_panel.InitialColor = Colors.Yellow;
        }


        private void _panel_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Background = new SolidColorBrush(e.NewValue);
        }

        private void _panel_ColorApplied(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Background=new SolidColorBrush(e.OldValue);
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            Background = new SolidColorBrush(e.NewValue);
            System.Diagnostics.Debug.WriteLine(_picker.ColorString);
        }
    }
}
