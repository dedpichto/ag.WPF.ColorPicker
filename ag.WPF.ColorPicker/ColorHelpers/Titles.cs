using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ag.WPF.ColorPicker.ColorHelpers
{
    public class Titles
    {
        public static readonly DependencyProperty TitleApplyProperty = DependencyProperty.RegisterAttached("TitleApply", typeof(string), typeof(Titles), new PropertyMetadata("Apply"));

        public static string GetTitleApply(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(TitleApplyProperty);

        public static void SetTitleApply(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(TitleApplyProperty, value);
    }
}
