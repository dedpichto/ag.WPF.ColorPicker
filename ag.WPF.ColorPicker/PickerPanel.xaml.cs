using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ag.WPF.ColorPicker
{
    public partial class PickerPanel : Window, INotifyPropertyChanged
    {
        private System.Windows.Media.Color _selectedColor;
        private LinearGradientBrush _scopeBrush;
        private double _previewLeft;
        private double _previewTop;
        private ImageSource _previewSource;
        private Cursor _cursor;

        #region Public properties
        /// <summary>
        /// Gets or sets a border brush for currently selected color sample.
        /// </summary>
        public LinearGradientBrush ScopeBrush
        {
            get => _scopeBrush;
            set
            {
                if (_scopeBrush != null && _scopeBrush.Equals(value)) return; _scopeBrush = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public System.Windows.Media.Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor == value) return; _selectedColor = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets preview rectangle Left.
        /// </summary>
        public double PreviewLeft
        {
            get => _previewLeft;
            set
            {
                if (_previewLeft == value) return; _previewLeft = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets preview rectangle Top.
        /// </summary>
        public double PreviewTop
        {
            get => _previewTop;
            set
            {
                if (_previewTop == value) return; _previewTop = value; OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the ImageSource for preview rectangle content.
        /// </summary>
        public ImageSource PreviewSource
        {
            get => _previewSource;
            set
            {
                if (_previewSource != null && _previewSource.Equals(value)) return; _previewSource = value; OnPropertyChanged();
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// Creates new instance of PickerPanel.
        /// </summary>
        public PickerPanel()
        {
            InitializeComponent();
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Occurrs when window's content is rendered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Topmost = false;
        }
        #endregion

        #region Private event handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var stream = Application.GetResourceStream(new Uri("pack://application:,,,/ag.WPF.ColorPicker;component/dropper.cur"));
            if (stream == null || stream.Stream == null) return;
            _cursor = new Cursor(stream.Stream);
            if (_cursor != null)
                _canvas.Cursor = _cursor;
            _image.Source = GetBitmap();
            var screenPoint = PointToScreen(Mouse.GetPosition(this));
            PreviewSource = GetBitmap((int)screenPoint.X, (int)screenPoint.Y);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            var pt = e.GetPosition(this);
            if (pt.X < 120 + 2 && pt.Y < 120 + 2)
                PreviewTop = SystemParameters.VirtualScreenHeight - 120;
            else
                PreviewTop = 0;
            var screenPoint = PointToScreen(pt);
            PreviewSource = GetBitmap((int)screenPoint.X, (int)screenPoint.Y);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                DialogResult = false;
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                var pt = Mouse.GetPosition(this);
                var screenPoint = PointToScreen(pt);
                screenPoint.Y--;
                UnsafeNativeMethods.SetCursorPosition((int)screenPoint.X, (int)screenPoint.Y);
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                var pt = Mouse.GetPosition(this);
                var screenPoint = PointToScreen(pt);
                screenPoint.Y++;
                UnsafeNativeMethods.SetCursorPosition((int)screenPoint.X, (int)screenPoint.Y);
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                var pt = Mouse.GetPosition(this);
                var screenPoint = PointToScreen(pt);
                screenPoint.X--;
                UnsafeNativeMethods.SetCursorPosition((int)screenPoint.X, (int)screenPoint.Y);
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                var pt = Mouse.GetPosition(this);
                var screenPoint = PointToScreen(pt);
                screenPoint.X++;
                UnsafeNativeMethods.SetCursorPosition((int)screenPoint.X, (int)screenPoint.Y);
            }
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DialogResult = true;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            DialogResult = true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            _cursor?.Dispose();
        }
        #endregion

        #region Private procedures
        private System.Windows.Media.Color[] GetScopeColors(int x, int y)
        {
            var colors = new System.Windows.Media.Color[3];
            var bmp = new Bitmap(3, 3);
            var bounds = new System.Drawing.Rectangle(x - 1, y - 1, 3, 3);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

            }

            var clr1 = bmp.GetPixel(0, 1);
            colors[0] = System.Windows.Media.Color.FromArgb(clr1.A, (byte)~clr1.R, (byte)~clr1.G, (byte)~clr1.B);

            var clr2 = bmp.GetPixel(1, 1);
            colors[1] = System.Windows.Media.Color.FromArgb(clr2.A, (byte)~clr2.R, (byte)~clr2.G, (byte)~clr2.B);
            var clr3 = bmp.GetPixel(1, 1);
            colors[2] = System.Windows.Media.Color.FromArgb(clr3.A, (byte)~clr3.R, (byte)~clr3.G, (byte)~clr3.B);

            return colors;
        }

        private System.Windows.Media.Color GetSelectedColor(int x, int y)
        {
            var bmp = new Bitmap(1, 1);
            var bounds = new System.Drawing.Rectangle(x, y, 1, 1);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

            }
            var clr = bmp.GetPixel(0, 0);
            return System.Windows.Media.Color.FromArgb(clr.A, clr.R, clr.G, clr.B);
        }

        private ImageSource GetBitmap(int x, int y)
        {
            var bmp = new Bitmap(9, 9);
            var bounds = new System.Drawing.Rectangle(x - 4, y - 4, 9, 9);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

            }
            SelectedColor = GetSelectedColor(x, y);
            var colors = GetScopeColors(x, y);
            ScopeBrush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0.5),
                EndPoint = new System.Windows.Point(1, 0.5),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(colors[0], 0),
                    new GradientStop(colors[1], 0.5),
                    new GradientStop(colors[2], 1)
                }
            };
            return ToWpfBitmap(bmp);
        }

        private ImageSource GetBitmap()
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var width = (int)(SystemParameters.VirtualScreenWidth * dpi.DpiScaleX);
            var height = (int)(SystemParameters.VirtualScreenHeight * dpi.DpiScaleY);

            var bmp = new Bitmap(width, height);
            var bounds = new System.Drawing.Rectangle(0, 0, width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size);

            }
            return ToWpfBitmap(bmp);
        }

        private BitmapSource ToWpfBitmap(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);

            stream.Position = 0;
            var result = new BitmapImage();
            result.BeginInit();
            // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
            // Force the bitmap to load right now so we can dispose the stream.
            result.CacheOption = BitmapCacheOption.OnLoad;
            result.StreamSource = stream;
            result.EndInit();
            result.Freeze();

            return result;
        }
        #endregion
    }
}
