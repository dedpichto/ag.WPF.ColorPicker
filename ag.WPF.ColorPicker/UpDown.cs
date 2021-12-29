using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ag.WPF.ColorPicker
{
    #region Named parts
    [TemplatePart(Name = ElementText, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementButtonUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = ElementButtonDown, Type = typeof(RepeatButton))]
    #endregion
    public sealed class UpDown : Control
    {
        private enum CurrentKey
        {
            None,
            Number,
            Delete,
            Back
        }

        private struct CurrentPosition
        {
            public CurrentKey Key;
            public int Offset;
        }

        #region Constants
        private const string ElementText = "PART_Text";
        private const string ElementButtonUp = "PART_Up";
        private const string ElementButtonDown = "PART_Down";
        #endregion

        #region Elements
        private TextBox _textBox;
        private RepeatButton _upButton;
        private RepeatButton _downButton;
        #endregion

        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(UpDown),
                new FrameworkPropertyMetadata(0m, OnValueChanged, ConstraintValue));
        /// <summary>
        /// The identifier of the <see cref="MaxValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(decimal), typeof(UpDown),
                new FrameworkPropertyMetadata(100m, OnMaxValueChanged, CoerceMaximum));
        /// <summary>
        /// The identifier of the <see cref="MinValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(decimal), typeof(UpDown),
                new FrameworkPropertyMetadata(0m, OnMinValueChanged));
        /// <summary>
        /// The identifier of the <see cref="Step"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(decimal), typeof(UpDown),
                new FrameworkPropertyMetadata(1m, OnStepChanged, CoerceStep));
        /// <summary>
        /// The identifier of the <see cref="NegativeForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NegativeForegroundProperty = DependencyProperty.Register(nameof(NegativeForeground), typeof(SolidColorBrush), typeof(UpDown),
                new FrameworkPropertyMetadata(Brushes.Red, OnNegativeForegroundChanged));
        /// <summary>
        /// The identifier of the <see cref="DecimalPlaces"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register(nameof(DecimalPlaces), typeof(uint), typeof(UpDown),
                new FrameworkPropertyMetadata((uint)0, OnDecimalPlacesChanged));
        /// <summary>
        /// The identifier of the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UpDown),
                new FrameworkPropertyMetadata(true, OnIsReadOnlyChanged));
        /// <summary>
        /// The identifier of the <see cref="UseGroupSeparator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseGroupSeparatorProperty = DependencyProperty.Register(nameof(UseGroupSeparator), typeof(bool), typeof(UpDown),
                new FrameworkPropertyMetadata(true, OnUseGroupSeparatorChanged));
        #endregion

        private CurrentPosition _Position;

        static UpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UpDown), new FrameworkPropertyMetadata(typeof(UpDown)));
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the value that indicates whether group separator is used for number formatting
        /// </summary>
        public bool UseGroupSeparator
        {
            get => (bool)GetValue(UseGroupSeparatorProperty);
            set => SetValue(UseGroupSeparatorProperty, value);
        }

        /// <summary>
        /// Gets or sets the value that indicates whether UpDown is in read-only state
        /// </summary>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// Gets or sets the value that indicates the count of decimal digits shown at UpDown
        /// </summary>
        public uint DecimalPlaces
        {
            get => (uint)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }
        /// <summary>
        /// Gets or sets the Brush to apply to the text contents of UpDown
        /// </summary>
        public SolidColorBrush NegativeForeground
        {
            get => (SolidColorBrush)GetValue(NegativeForegroundProperty);
            set => SetValue(NegativeForegroundProperty, value);
        }
        /// <summary>
        /// Gets or sets the value to increment or decrement UpDown when the up or down buttons are clicked
        /// </summary>
        public decimal Step
        {
            get => (decimal)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }
        /// <summary>
        /// Gets or sets the minimum allowed value of UpDown
        /// </summary>
        public decimal MinValue
        {
            get => (decimal)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }
        /// <summary>
        /// Gets or sets the maximum allowed value of UpDown
        /// </summary>
        public decimal MaxValue
        {
            get => (decimal)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        /// <summary>
        /// Gets or sets the value of UpDown
        /// </summary>
        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        #endregion

        #region Routed events
        /// <summary>
        /// Occurs when the <see cref="UseGroupSeparator"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> UseGroupSeparatorChanged
        {
            add => AddHandler(UseGroupSeparatorChangedEvent, value);
            remove => RemoveHandler(UseGroupSeparatorChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="UseGroupSeparatorChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent UseGroupSeparatorChangedEvent = EventManager.RegisterRoutedEvent("UseGroupSeparatorChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(UpDown));

        /// <summary>
        /// Occurs when the <see cref="DecimalPlaces"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<uint> DecimalPlacesChanged
        {
            add => AddHandler(DecimalPlacesChangedEvent, value);
            remove => RemoveHandler(DecimalPlacesChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="DecimalPlacesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent DecimalPlacesChangedEvent = EventManager.RegisterRoutedEvent("DecimalPlacesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<uint>), typeof(UpDown));

        /// <summary>
        /// Occurs when the <see cref="NegativeForeground"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<SolidColorBrush> NegativeForegroundChanged
        {
            add => AddHandler(NegativeForegroundChangedEvent, value);
            remove => RemoveHandler(NegativeForegroundChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="NegativeForegroundChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent NegativeForegroundChangedEvent = EventManager.RegisterRoutedEvent("NegativeForegroundChanged",
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<SolidColorBrush>), typeof(UpDown));
        /// <summary>
        /// Occurs when the <see cref="Step"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> StepChanged
        {
            add => AddHandler(StepChangedEvent, value);
            remove => RemoveHandler(StepChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="StepChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent StepChangedEvent = EventManager.RegisterRoutedEvent("StepChanged",
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(UpDown));
        /// <summary>
        /// Occurs when the <see cref="MinValue"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> MinValueChanged
        {
            add => AddHandler(MinValueChangedEvent, value);
            remove => RemoveHandler(MinValueChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="MinValueChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent MinValueChangedEvent = EventManager.RegisterRoutedEvent("MinValueChanged",
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(UpDown));
        /// <summary>
        /// Occurs when the <see cref="MaxValueChanged"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> MaxValueChanged
        {
            add => AddHandler(MaxValueChangedEvent, value);
            remove => RemoveHandler(MaxValueChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="MaxValueChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent MaxValueChangedEvent = EventManager.RegisterRoutedEvent("MaxValueChanged",
            RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(UpDown));
        /// <summary>
        /// Occurs when the <see cref="Value"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }
        /// <summary>
        /// Identifies the <see cref="ValueChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(UpDown));
        #endregion

        #region Callback procedures
        private static void OnIsReadOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        /// <summary>
        /// Invoked just before the <see cref="UseGroupSeparatorChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        private void OnUseGroupSeparatorChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = UseGroupSeparatorChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnUseGroupSeparatorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.OnUseGroupSeparatorChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="NegativeForegroundChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old foreground</param>
        /// <param name="newValue">New foreground</param>
        private void OnNegativeForegroundChanged(SolidColorBrush oldValue, SolidColorBrush newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<SolidColorBrush>(oldValue, newValue)
            {
                RoutedEvent = NegativeForegroundChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnNegativeForegroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.OnNegativeForegroundChanged((SolidColorBrush)e.OldValue, (SolidColorBrush)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="DecimalPlacesChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old decimal digits count</param>
        /// <param name="newValue">New decimal digits count</param>
        private void OnDecimalPlacesChanged(uint oldValue, uint newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<uint>(oldValue, newValue)
            {
                RoutedEvent = DecimalPlacesChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnDecimalPlacesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.OnDecimalPlacesChanged((uint)e.OldValue, (uint)e.NewValue);
        }

        //private static object CoerceDecimalPlaces(DependencyObject d, object value)
        //{
        //    if (!(d is UpDown upd)) return value;
        //    var fraction = upd.Step - decimal.Truncate(upd.Step);
        //    var count = Convert.ToUInt32(value);
        //    var arr =
        //        fraction.ToString("f", CultureInfo.CurrentCulture)
        //            .Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray());
        //    if (arr.Length == 2 && arr[1].Length > count)
        //        count = (uint) arr[1].Length;
        //    return count;
        //}
        /// <summary>
        /// Invoked just before the <see cref="StepChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old step</param>
        /// <param name="newValue">New step</param>
        private void OnStepChanged(decimal oldValue, decimal newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue)
            {
                RoutedEvent = StepChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnStepChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.OnStepChanged(Convert.ToDecimal(e.OldValue), Convert.ToDecimal(e.NewValue));
        }

        private static object CoerceStep(DependencyObject d, object value)
        {
            if (!(d is UpDown upd)) return value;
            var step = Convert.ToDecimal(value);
            step = step < 0 ? Math.Abs(step) : step;
            var fraction = step - decimal.Truncate(step);
            var arr =
                fraction.ToString(CultureInfo.CurrentCulture)
                    .Split(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.ToCharArray());
            if (arr.Length == 2 && arr[1].Length > upd.DecimalPlaces)
                upd.DecimalPlaces = (uint)arr[1].Length;
            return step;
        }
        /// <summary>
        /// Invoked just before the <see cref="MinValueChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old min value</param>
        /// <param name="newValue">New min value</param>
        private void OnMinValueChanged(decimal oldValue, decimal newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue)
            {
                RoutedEvent = MinValueChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnMinValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.CoerceValue(MaxValueProperty);
            upd.CoerceValue(ValueProperty);
            upd.OnMinValueChanged(Convert.ToDecimal(e.OldValue), Convert.ToDecimal(e.NewValue));
        }
        /// <summary>
        /// Invoked just before the <see cref="MaxValueChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old max value</param>
        /// <param name="newValue">New max value</param>
        private void OnMaxValueChanged(decimal oldValue, decimal newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue)
            {
                RoutedEvent = MaxValueChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnMaxValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.CoerceValue(ValueProperty);
            upd.OnMaxValueChanged(Convert.ToDecimal(e.OldValue), Convert.ToDecimal(e.NewValue));
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            var max = Convert.ToDecimal(value);
            if (!(d is UpDown upd)) return value;
            return max < upd.MinValue ? upd.MinValue : value;
        }
        /// <summary>
        /// Invoked just before the <see cref="ValueChanged"/> event is raised on UpDown
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        private void OnValueChanged(decimal oldValue, decimal newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue)
            {
                RoutedEvent = ValueChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UpDown upd)) return;
            upd.OnValueChanged(Convert.ToDecimal(e.OldValue), Convert.ToDecimal(e.NewValue));
        }

        private static object ConstraintValue(DependencyObject d, object value)
        {
            var newValue = Convert.ToDecimal(value);
            if (!(d is UpDown upd)) return value;
            if (newValue < upd.MinValue) return upd.MinValue;
            return newValue > upd.MaxValue ? upd.MaxValue : value;
        }

        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //var bd = new Binding("Value")
            //{
            //    Mode = BindingMode.TwoWay,
            //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UpDown), 1),
            //    Converter = new TextToDecimalConverter(),
            //    ConverterParameter = this,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //};

            if (_textBox != null)
            {
                _textBox.GotFocus -= textBox_GotFocus;
                _textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
                _textBox.PreviewMouseRightButtonUp -= textBox_PreviewMouseRightButtonUp;
                _textBox.TextChanged -= textBox_TextChanged;
                //BindingOperations.ClearAllBindings(_Text);
            }
            _textBox = GetTemplateChild(ElementText) as TextBox;
            if (_textBox != null)
            {
                _textBox.GotFocus += textBox_GotFocus;
                _textBox.PreviewKeyDown += textBox_PreviewKeyDown;
                _textBox.PreviewMouseRightButtonUp += textBox_PreviewMouseRightButtonUp;
                _textBox.TextChanged += textBox_TextChanged;
                //_Text.SetBinding(TextBox.TextProperty, bd);
            }

            if (_downButton != null)
            {
                _downButton.Click -= downButton_Click;
            }
            _downButton = GetTemplateChild(ElementButtonDown) as RepeatButton;
            if (_downButton != null)
            {
                _downButton.Click += downButton_Click;
            }

            if (_upButton != null)
            {
                _upButton.Click -= upButton_Click;
            }
            _upButton = GetTemplateChild(ElementButtonUp) as RepeatButton;
            if (_upButton != null)
            {
                _upButton.Click += upButton_Click;
            }
        }
        #endregion

        #region Private event handlers
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            addStep(true);
            if (!_textBox.IsFocused)
                _textBox.Focus();
            else
                _textBox.SelectAll();
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            addStep(false);
            if (!_textBox.IsFocused)
                _textBox.Focus();
            else
                _textBox.SelectAll();
        }

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBox.SelectAll();
            e.Handled = true;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsReadOnly)
            {
                BindingOperations.GetMultiBindingExpression(_textBox, TextBox.TextProperty).UpdateSource();
            }
            switch (_Position.Key)
            {
                case CurrentKey.Number:
                    if (DecimalPlaces == 0 || _textBox.Text.Length == 0) return;
                    var position = _Position.Offset == -1 ? 1 : _textBox.Text.Length - _Position.Offset;
                    // ReSharper disable once RedundantCheckBeforeAssignment
                    if (_textBox.CaretIndex != position)
                        _textBox.CaretIndex = position;
                    break;
            }
        }

        private void textBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;
                return;
            }
            while (true)
            {
                _Position = new CurrentPosition();
                switch (e.Key)
                {
                    case Key.Up:
                        addStep(true);
                        _textBox.SelectAll();
                        e.Handled = true;
                        break;
                    case Key.Down:
                        addStep(false);
                        _textBox.SelectAll();
                        e.Handled = true;
                        break;
                    case Key.Delete:
                        if (IsReadOnly)
                        {
                            e.Handled = true;
                            break;
                        }
                        if ((_textBox.SelectionLength == _textBox.Text.Length) || (_textBox.CaretIndex == 0 && _textBox.Text.Length == 1))
                        {
                            Value = MinValue;
                            e.Handled = true;
                            break;
                        }
                        if (_textBox.CaretIndex < _textBox.Text.Length)
                        {
                            if (DecimalPlaces > 0 &&
                                _textBox.CaretIndex ==
                                _textBox.Text.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
                                    StringComparison.Ordinal))
                            {
                                _textBox.CaretIndex++;
                                continue;
                            }
                            if (_textBox.Text[_textBox.CaretIndex] == CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator[0])
                            {
                                _textBox.CaretIndex++;
                                continue;
                            }
                        }
                        break;
                    case Key.Back:
                        if (IsReadOnly)
                        {
                            e.Handled = true;
                            break;
                        }
                        if ((_textBox.SelectionLength == _textBox.Text.Length) || (_textBox.CaretIndex == 1 && _textBox.Text.Length == 1))
                        {
                            Value = MinValue;
                            e.Handled = true;
                            break;
                        }
                        if (_textBox.CaretIndex != 0)
                        {
                            if (DecimalPlaces > 0 &&
                                _textBox.CaretIndex ==
                                _textBox.Text.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
                                    StringComparison.Ordinal) + 1)
                            {
                                _textBox.CaretIndex--;
                                continue;
                            }
                            if (_textBox.Text[_textBox.CaretIndex - 1] == CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator[0])
                            {
                                _textBox.CaretIndex--;
                                continue;
                            }
                        }
                        break;
                    case Key.D0:
                    case Key.NumPad0:
                    case Key.D1:
                    case Key.NumPad1:
                    case Key.D2:
                    case Key.NumPad2:
                    case Key.D3:
                    case Key.NumPad3:
                    case Key.D4:
                    case Key.NumPad4:
                    case Key.D5:
                    case Key.NumPad5:
                    case Key.D6:
                    case Key.NumPad6:
                    case Key.D7:
                    case Key.NumPad7:
                    case Key.D8:
                    case Key.NumPad8:
                    case Key.D9:
                    case Key.NumPad9:
                        if (IsReadOnly || (Value == MaxValue && _textBox.SelectionLength != _textBox.Text.Length))
                        {
                            e.Handled = true;
                            break;
                        }
                        _Position.Key = CurrentKey.Number;
                        //{
                        //    var temp = _Text.Text.ToCharArray().ToList();
                        //    if (_Text.SelectionLength > 0)
                        //    {
                        //        for (var i = _Text.SelectionStart + _Text.SelectionLength - 1; i >= _Text.SelectionStart; i--)
                        //        {
                        //            temp.RemoveAt(i);
                        //        }
                        //    }
                        //    temp.Insert(_Text.SelectionStart, charFromNumberKey(e.Key));
                        //    Value = (decimal)new TextToDecimalConverter().ConvertBack(new string(temp.ToArray()), null, this,
                        //        CultureInfo.CurrentCulture);
                        //    e.Handled = true;
                        //}
                        if (DecimalPlaces > 0)
                        {
                            var sepPos = _textBox.Text.IndexOf(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal);

                            if ((_textBox.SelectionStart + _textBox.SelectionLength) <= sepPos)
                            {
                                _Position.Offset = _textBox.SelectionLength == _textBox.Text.Length
                                    ? -1
                                    : _textBox.Text.Length - (_textBox.SelectionLength + _textBox.SelectionStart);
                            }
                            else if (_textBox.SelectionStart < sepPos && (_textBox.SelectionStart + _textBox.SelectionLength) > sepPos)
                            {
                                _Position.Offset = _textBox.SelectionLength == _textBox.Text.Length
                                    ? -1
                                    : _textBox.Text.Length - (_textBox.SelectionLength + _textBox.SelectionStart) - 1;
                                _textBox.Text = _textBox.Text.Remove(_textBox.SelectionStart, _textBox.SelectionLength).Insert(_textBox.SelectionStart, charFromNumberKey(e.Key)).Insert(_textBox.SelectionStart + 1, CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                                e.Handled = true;
                            }
                            else if (_textBox.SelectionStart > sepPos && _textBox.SelectionStart < _textBox.Text.Length)
                            {
                                if (_textBox.SelectionLength == 0)
                                {
                                    _Position.Offset = _textBox.Text.Length - _textBox.SelectionStart - 1;
                                    _textBox.Text = _textBox.Text.Remove(_textBox.SelectionStart, 1).Insert(_textBox.SelectionStart, charFromNumberKey(e.Key));
                                    e.Handled = true;
                                }
                            }
                        }
                        break;
                    case Key.Tab:
                    case Key.Right:
                    case Key.Left:
                    case Key.Home:
                    case Key.End:
                    case Key.Escape:
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        if (IsReadOnly)
                        {
                            e.Handled = true;
                            break;
                        }
                        if ((_textBox.Text.Any(c => c == '-')) || (_textBox.CaretIndex > 0) ||
                            (_textBox.SelectionLength == _textBox.Text.Length) || (MinValue >= 0))
                        {
                            e.Handled = true;
                        }
                        break;
                    case Key.OemPeriod:
                    case Key.Decimal:
                        if (IsReadOnly)
                        {
                            e.Handled = true;
                            break;
                        }
                        if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." && DecimalPlaces > 0 &&
                            _textBox.CaretIndex == _textBox.Text.IndexOf('.') &&
                            _textBox.SelectionLength != _textBox.Text.Length)
                        {
                            _textBox.Select(_textBox.CaretIndex + 1, 0);
                        }
                        e.Handled = true;
                        break;
                    case Key.OemComma:
                        if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," && DecimalPlaces > 0 &&
                            _textBox.CaretIndex == _textBox.Text.IndexOf(',') &&
                            _textBox.SelectionLength != _textBox.Text.Length)
                        {
                            _textBox.Select(_textBox.CaretIndex + 1, 0);
                        }
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
                break;
            }
        }
        #endregion

        #region Private procedures

        private string charFromNumberKey(Key key)
        {
            switch (key)
            {
                case Key.D0:
                case Key.NumPad0:
                    return "0";
                case Key.D1:
                case Key.NumPad1:
                    return "1";
                case Key.D2:
                case Key.NumPad2:
                    return "2";
                case Key.D3:
                case Key.NumPad3:
                    return "3";
                case Key.D4:
                case Key.NumPad4:
                    return "4";
                case Key.D5:
                case Key.NumPad5:
                    return "5";
                case Key.D6:
                case Key.NumPad6:
                    return "6";
                case Key.D7:
                case Key.NumPad7:
                    return "7";
                case Key.D8:
                case Key.NumPad8:
                    return "8";
                case Key.D9:
                case Key.NumPad9:
                    return "9";
            }
            return "";
        }

        private void addStep(bool plus)
        {
            if (plus)
            {
                if (Value + Step <= MaxValue)
                    Value += Step;
            }
            else if (Value - Step >= MinValue)
            {
                Value -= Step;
            }
        }
        #endregion
    }

    public class UpDownForegroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is decimal decimalValue) || !(values[1] is Brush foregroundBrush) || !(values[2] is Brush negativeBrush)) return null;
            return decimalValue >= 0 ? foregroundBrush : negativeBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UpDownTextToValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is decimal decimalValue) || !(values[1] is uint decimalPlaces) || !(values[2] is bool useSeparator)) return "";
            var partInt = decimal.Truncate(decimalValue);
            var partFraction =
                Math.Abs(decimal.Truncate((decimalValue - partInt) * (int)Math.Pow(10.0, decimalPlaces)));
            var formatInt = useSeparator ? "#" + culture.NumberFormat.NumberGroupSeparator + "##0" : "##0";
            var formatFraction = new string('0', (int)decimalPlaces);
            var stringInt = partInt.ToString(formatInt);
            if (decimalValue < 0 && partInt == 0)
                stringInt = $"-{stringInt}";
            var result = decimalPlaces > 0
                ? $"{stringInt}{culture.NumberFormat.NumberDecimalSeparator}{partFraction.ToString(formatFraction)}"
                : stringInt;
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is string stringValue)) return null;
            if (string.IsNullOrEmpty(stringValue)) stringValue = "0";
            stringValue = stringValue.Replace(culture.NumberFormat.NumberGroupSeparator, "");
            return new object[] { decimal.Parse(stringValue, NumberStyles.Any) };
        }
    }
}
