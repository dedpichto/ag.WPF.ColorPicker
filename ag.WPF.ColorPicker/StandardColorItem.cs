using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.ColorPicker
{
    internal class StandardColorItem : StackPanel
    {
        public Color Color { get; private set; }
        public string ColorName { get; private set; }
        public StandardColorItem(Color color, string colorName)
        {
            Color = color;
            ColorName = colorName;
            Cursor = Cursors.Hand;
            Orientation = Orientation.Horizontal;
            var rectangle = new Rectangle
            {
                Fill = new SolidColorBrush(color),
                Width = 20,
                Height = 16,
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            var textBlock = new TextBlock
            {
                Text = colorName,
                Margin = new Thickness(8, 2, 2, 2),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            Children.Add(rectangle);
            Children.Add(textBlock);
        }
    }
}
