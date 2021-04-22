using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace McsfRestoreTool.Validtions
{
    public class PortValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string pattern = @"^([0-9]*)$";
            if (value == null)
            {
                return new ValidationResult(false, "can not empty");
            }
            string field = value.ToString().Trim();
            if (String.IsNullOrEmpty(field))
            {
                return new ValidationResult(false, "can not empty");
            }
            var input = field.Substring(field.Length - 1);
            if (!Regex.IsMatch(input, pattern))
            {
                return new ValidationResult(false, "please input  number");
            }
            if (field.Length < 4)
            {
                return new ValidationResult(false, "the shotest length is four");
            }
            int iValue;
            var ret = int.TryParse(field, out iValue);
            if (ret && (iValue > 49151 || iValue < 1024))
            {
                return new ValidationResult(false, "range: 1024-49151");
            }
            return ValidationResult.ValidResult;
        }
    }
}

