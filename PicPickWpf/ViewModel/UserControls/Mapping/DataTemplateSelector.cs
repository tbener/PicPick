using System.Windows;
using System.Windows.Controls;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingFileDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MappingFileViewModel)
            {
                MappingFileViewModel f = item as MappingFileViewModel;
                FrameworkElement element = container as FrameworkElement;
                switch (f.DestinationFileInfo.Status)
                {
                    case Core.FILE_STATUS.NONE:
                        // display only source file (that's the default)
                        break;
                    case Core.FILE_STATUS.COPIED:
                        // display something like: [Destination File Name] (from: [Source Full Name])
                        return element.FindResource("CopiedFile") as DataTemplate;
                    case Core.FILE_STATUS.SKIPPED:
                        // display only source file (default)
                        break;
                    case Core.FILE_STATUS.ERROR:
                        // Source full name (error message...)
                        return element.FindResource("ErrorFile") as DataTemplate;
                    default:
                        break;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
