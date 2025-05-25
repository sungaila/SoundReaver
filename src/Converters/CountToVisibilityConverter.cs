using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Sungaila.SoundReaver.Converters
{
    internal partial class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not int count)
                return DependencyProperty.UnsetValue;

            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}