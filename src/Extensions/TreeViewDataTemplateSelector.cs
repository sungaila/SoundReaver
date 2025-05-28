using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.ViewModels;

namespace Sungaila.SoundReaver.Extensions
{
    internal partial class TreeViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? CategoryTemplate { get; set; }

        public DataTemplate? TrackTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            if (item is CategoryViewModel)
                return CategoryTemplate;

            if (item is TrackViewModel)
                return TrackTemplate;

            return null;
        }
    }
}