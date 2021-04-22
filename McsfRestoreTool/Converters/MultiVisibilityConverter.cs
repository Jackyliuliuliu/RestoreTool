using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.Converters
{
    public class MultiVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                McsfRestoreLogger.WriteLog("[MultiVisibilityConverter]: values is null or lenght is less 2.");
                return Visibility.Collapsed;
            }
            if (values.Length == 2)
            {
                if ((bool)values[0] || (bool)values[1])
                {
                    McsfRestoreLogger.WriteLog(string.Format("[MultiVisibilityConverter]: values 0 is {0} and 1 is {1}", values[0], values[1]));
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            McsfRestoreLogger.WriteLog("[MultiVisibilityConverter]: values length is " + values.Length);
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
