using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitPlugin.Views.Converters
{
    internal class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool isVisible = visibility == Visibility.Visible;

                if (parameter != null && parameter.ToString() == "Inverse")
                {
                    return !isVisible;
                }

                return isVisible;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
