using ISTQB_PL.Views;
using System;
using Xamarin.Forms;

namespace ISTQB_PL
{
    public partial class AppShell : Shell
    {
        public AppShell ()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            if (!Application.Current.Properties.ContainsKey("MySwitchValue"))
            {
                Application.Current.Properties["MySwitchValue"] = false;
            }
            if (!Application.Current.Properties.ContainsKey("SliderValue"))
            {
                Application.Current.Properties["SliderValue"] = 6;
            }
            if (!Application.Current.Properties.ContainsKey("FontSize"))
            {
                Application.Current.Properties["FontSize"] = 18;
            }
            Application.Current.SavePropertiesAsync();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            //await Current.GoToAsync($"{nameof(LoginPage)}");
            await Navigation.PushAsync(new LoginPage());
        }
        private async void OnMenuItemSettingsClicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = false;
            await Navigation.PushAsync(new SettingsPage());
            //await Current.GoToAsync($"{nameof(SettingsPage)}");
        }
    }
}
