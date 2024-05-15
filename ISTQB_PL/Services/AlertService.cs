using ISTQB_PL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AlertService))]
namespace ISTQB_PL.Services
{
    public class AlertService : IAlertService
    {
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            // Android-specific alert code
            await Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
        public async Task<bool> DisplayAlertCommit(string title, string message, string commit, string cancel)
        {
            // Android-specific alert code
            return await Application.Current.MainPage.DisplayAlert(title, message, commit, cancel);
        }
    }
}
