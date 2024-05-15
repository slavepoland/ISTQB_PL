using ISTQB_PL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
namespace ISTQB_PL.Droid.Services
{
    public class AlertService : IAlertService
    {
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            // Android-specific alert code
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
