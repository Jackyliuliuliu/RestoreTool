using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UIH.Mcsf.Restore.Logger;

namespace McsfRestoreTool.Converters
{
    public class BoolToReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                McsfRestoreLogger.WriteLog("[BoolToReverseConverter]: value is null.");
                return true;
            }
            else
            {
                var ret = !(bool)value;
                McsfRestoreLogger.WriteLog("[BoolToReverseConverter]: ret is " + ret);
                return ret;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
