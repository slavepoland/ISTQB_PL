using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class OptionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double sliderValue)
            {
                return (int)Math.Round(sliderValue);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            {
                if (value is int optionValue)
                {
                    return (double)optionValue;
                }
                return 0.0;
            }
        }
    }
}
