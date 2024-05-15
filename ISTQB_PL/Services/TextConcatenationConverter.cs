using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class TextConcatenationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(parameter.ToString() == ".")
            {
                if (value is string bindingText && parameter is string additionalText)
                {
                    return bindingText + additionalText;
                }
            }
            if (parameter.ToString() == "P.")
            {
                if (value is string bindingText && parameter is string additionalText)
                {
                    return additionalText + bindingText;
                }
            }
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
