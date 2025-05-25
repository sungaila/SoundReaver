using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;

namespace Sungaila.SoundReaver.Converters
{
    internal partial class BoolToSymbolConverter : IValueConverter
    {
        public Symbol True { get; set; } = Symbol.Accept;

        public Symbol False { get; set; } = Symbol.Cancel;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool boolean)
                return DependencyProperty.UnsetValue;

            return boolean ? True : False;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
