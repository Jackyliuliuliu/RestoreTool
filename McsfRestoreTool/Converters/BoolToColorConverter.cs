using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                McsfRestoreLogger.WriteLog("[BoolToColorConverter]: value is null.");
                return false;
            }
            else
            {
                if ((bool)value)
                {
                    return "green";
                }
                else
                {
                    McsfRestoreLogger.WriteLog("[BoolToColorConverter]: value is ." + value);
                    return "red";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
