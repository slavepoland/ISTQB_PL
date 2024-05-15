using ISTQB_PL.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace ISTQB_PL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel ViewModel;
        //private SettingsViewModel _SettingsViewModelContext;

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }
        private int myFontSize;


        public LoginPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new LoginViewModel();
            if (Application.Current.Properties.ContainsKey("Login"))
            {
                EntryLogin.Text = Application.Current.Properties["Login"].ToString();
            }
            if (Application.Current.Properties.ContainsKey("Passwd"))
            {
                EntryPasswd.Text = Application.Current.Properties["Passwd"].ToString();
            }
            ChangeFontSize();
        }

        protected override bool OnBackButtonPressed()
        {
            // Tutaj możesz dodać swoją logikę kontrolującą zachowanie przycisku wstecz
            // Na przykład, wyświetl okno dialogowe, zapytaj użytkownika o potwierdzenie lub wykonaj inne czynności.

            // Zwróć true, jeśli chcesz zatrzymać domyślne zachowanie przycisku wstecz, w przeciwnym razie zwróć false.
            return true; // Domyślnie zwraca false
        }

        private void ChangeFontSize()
        {
            MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());

            LblLogin.FontSize = MyFontSize;
            LblPasswd.FontSize = MyFontSize;
            EntryLogin.FontSize = MyFontSize;
            EntryPasswd.FontSize = MyFontSize;
            BtnLogin.FontSize = MyFontSize;
        }

        //uruchomienie akcji logowania, i pokazanie ActivityIndicator
        private async void OnLoginClicked(object sender, EventArgs e) 
        {
            try
            {
                Application.Current.Properties["Login"] = EntryLogin.Text;
                Application.Current.Properties["Passwd"] = EntryPasswd.Text;
                await Application.Current.SavePropertiesAsync();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Dane logowania", $"Zapis danych się nie udał:{ex}", "Ok");
            }

            BtnLogin.IsVisible = false;
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;


            // Run a task on a new thread
            bool loginSuccessful = await Task.Run(() => LoginAsync());

            if (loginSuccessful)
            {
                // Navigate to the next page (replace with your actual navigation logic)
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");    
            }
            else
            {
                // Handle login failure, e.g., show an error message
                await DisplayAlert("Uwaga!!!", "Login lub hasło niepoprawne!!!", "Ok"); 
            }
            // if login unsuccesful, hide the loading indicator
            BtnLogin.IsVisible = true;
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }

        //metoda odpowiadająca za sprawdzenie loginu i hasła, uruchamiana w osobnym wątku
        private async Task<bool> LoginAsync() 
        {
            // Simulate a delay for the login process (replace with your actual login logic)
            Task<bool> result = Task.FromResult(false);

            // Simulate a successful login (you can replace this logic with actual authentication)
            if (EntryLogin.Text != null && EntryPasswd.Text != null)
            {
                ViewModel._username = EntryLogin.Text;
                ViewModel._password = EntryPasswd.Text;
                result = Task.Run(async () => await ViewModel.OnLoginClicked());
            }

            return await Task.FromResult(result).Result;
        }
    }
}