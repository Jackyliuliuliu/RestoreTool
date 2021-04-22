using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace McsfRestoreTool.Validtions
{
    public class EmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (null == value)
            {
                return new ValidationResult(false, null);
            }

            string field = value.ToString().Trim();

            if (String.IsNullOrEmpty(field))
            {
                return new ValidationResult(false, "can not empty.");
            }
            return field.Contains(@"\") ? new ValidationResult(false, "forbid backslash.") : ValidationResult.ValidResult;
        }
    }
}
