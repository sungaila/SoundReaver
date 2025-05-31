using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Sungaila.SoundReaver.Converters
{
    internal partial class DoubleToVolumeGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not double volume)
                return DependencyProperty.UnsetValue;

            return volume switch
            {
                0 => "\uE74F",      // Mute
                < 25 => "\uE992",   // Volume0
                < 50 => "\uE993",   // Volume1
                < 75 => "\uE994",   // Volume2
                _ => "\uE995"       // Volume3
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}