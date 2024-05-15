using ISTQB_PL.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        //public Command LoginCommand { get; }
        public string _username;
        public string _password;

        //private string UserName
        //{
        //    get => _username;
        //    set
        //    {
        //        SetProperty(ref _username, value, "username");
        //    }
        //}
        //private string Password
        //{
        //    get => _password;
        //    set
        //    {
        //        SetProperty(ref _password, value, "passwd");
        //    }
        //}
        //private string LastLogin
        //{
        //    get => _lastlogin;
        //    set
        //    {
        //        SetProperty(ref _lastlogin, value, "lastlogin");
        //    }
        //}

        public LoginViewModel()
        {
            Title = "Manual tester - logowanie";
            //LoginCommand = new Command(OnLoginClicked);
        }

        public async Task<bool> OnLoginClicked() //object obj
        {
            //check username & passwd
            bool result = await DataStore.GoogleSheetsLogin(_username, _password);

            if (result == true)
            {
                _ = Task.Run(async () =>
                await DataStore.GoogleLastLogin(DateTime.Now, _username));
            }
            return result;
        }
    }
}
