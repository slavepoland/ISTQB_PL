using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string userResponse = value?.ToString();
            if (!string.IsNullOrEmpty(userResponse) && userResponse.Equals("tak", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Green;
            }
            else if(!string.IsNullOrEmpty(userResponse) && userResponse.Equals("nie", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Red;
            }
            else if (!string.IsNullOrEmpty(userResponse) && userResponse.Equals("niewiem", StringComparison.OrdinalIgnoreCase))
            {//niewiem oznacza, że sprawdzanie odpowiedzi jest po zakończeniu egzaminu
                return Color.Yellow;
            }
            else
            {
                return Color.Transparent;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Color.Transparent; // Zmień na pożądany kolor dla niezaznaczonego elementu
        }
    }
}
