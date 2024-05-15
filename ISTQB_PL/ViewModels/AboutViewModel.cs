using ISTQB_PL.Services;
using ISTQB_PL.Views;
using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        //SettingsViewModel _BindingContext = new SettingsViewModel();
        //string fileContent;

        public AboutViewModel()
        {
            Title = "Manual Tester - FL";
            //OpenItemsPageCommand =
            //new Command(() => { Shell.Current.GoToAsync($"ItemsPage"); });

            //OpenExamPageCommand =
            //new Command(() => { Shell.Current.GoToAsync($"//{nameof(CarouselMainPage)}"); }); // TestPage

            //OpenSylabusPageCommand = (ICommand)OpenSylabus();

        }

        //private async Task<Command> OpenSylabus()
        //{
        //    //ICommand OpenSylabusPageCommand;

        //    var alertService = DependencyService.Get<IAlertService>();
        //    await alertService.DisplayAlert("Info", "STRONA W BUDOWIE: Tutaj pojawi się treść Sylabusa.", "OK");

        //    return null;
        //}

        //public ICommand OpenItemsPageCommand { get; }
        //public ICommand OpenExamPageCommand { get; }
        //public ICommand OpenSylabusPageCommand { get; }
    }
}