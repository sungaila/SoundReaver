using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace Sungaila.SoundReaver.Converters
{
    internal partial class DoubleToTimeSpanFormattedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not double position)
                return DependencyProperty.UnsetValue;

            return TimeSpan.FromMilliseconds(position).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}