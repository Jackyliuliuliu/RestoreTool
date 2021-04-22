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
    public class BoolToSuccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                McsfRestoreLogger.WriteLog("[BoolToSuccessConverter]: value is null.");
                return false;
            }
            else
            {
                if ((bool)value)
                {
                    return "Success";
                }
                else
                {
                    McsfRestoreLogger.WriteLog("[BoolToSuccessConverter]: value is ." + value);
                    return "Failed";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
