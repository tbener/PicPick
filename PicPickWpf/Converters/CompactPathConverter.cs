using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using TalUtils;

namespace PicPick.Converters
{
    public sealed class CompactPathConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert bool or Nullable bool to Visibility
        /// </summary>
        /// <param name="value">path to display</param>
        /// <param name="targetType">String</param>
        /// <param name="parameter">Width to adjust the path to</param>
        /// <param name="culture">null</param>
        /// <returns>Compacted path</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value[0].ToString();
            var element = ((System.Windows.Controls.TextBlock)value[1]);
            Font font = new Font(element.FontFamily.ToString(), (float)element.FontSize, System.Drawing.FontStyle.Regular);
            int width = System.Convert.ToInt32(element.ActualWidth * 1.2) ;

            TextRenderer.MeasureText(path, font, new System.Drawing.Size(width, 0), TextFormatFlags.ModifyString | TextFormatFlags.PathEllipsis);

            int pos = path.IndexOf('\0');
            if (pos >= 0)
                path = path.Substring(0, pos);

            return path;
        }

        /// <summary>
        /// Cannot convert back
        /// </summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
