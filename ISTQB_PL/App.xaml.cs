using ISTQB_PL.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using MarcTron.Plugin;


namespace ISTQB_PL
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Register<MockDataStore>();
            DependencyService.Register<SylabusDataStore>();
            DependencyService.Register<GlossaryDataStore>();
            DependencyService.Register<SylabusWersjaDataStore>();

            CrossMTAdmob.Current.UserPersonalizedAds = true;
            CrossMTAdmob.Current.ComplyWithFamilyPolicies = true;
            CrossMTAdmob.Current.UseRestrictedDataProcessing = true;
            // Ustawia obsługę zdarzenia zmiany trybu jasnego/ciemnego
            App.Current.RequestedThemeChanged += (s, a) =>
            {
                // Tutaj możesz dostosować dodatkowe style lub reakcje na zmianę trybu
                UpdateStylesBasedOnTheme();
            };

            MainPage = new AppShell();
        }

        // Metoda do dostosowywania stylów na podstawie trybu jasnego/ciemnego
        private void UpdateStylesBasedOnTheme()
        {
            // Pobierz aktualny tryb systemowy
            var theme = Current.RequestedTheme;

            // Przykładowe dostosowanie stylów dla Label
            if (theme == OSAppTheme.Dark)
            {
                // Tryb ciemny
                Resources["JasneTlo"] = Color.FromHex("#000000");
                Resources["JasnyTekst"] = Color.FromHex("#FFFFFF");
            }
            else
            {
                // Tryb jasny
                Resources["JasneTlo"] = Color.FromHex("#FFFFFF");
                Resources["JasnyTekst"] = Color.FromHex("#000000");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            var currentTheme = AppInfo.RequestedTheme;
            var savedTheme = Preferences.Get("AppTheme", string.Empty);

            if (currentTheme.ToString() != savedTheme.ToString())
            {    
                // Zapisz aktualny tryb do ustawień
                Preferences.Set("AppTheme", currentTheme.ToString());
            }
            UpdateStylesBasedOnTheme();
        }

        protected override void OnSleep()
        {
            //base.OnSleep();
            //UpdateStylesBasedOnTheme();
        }

        protected override void OnResume()
        {
            base.OnResume();
            
            // Sprawdź zmianę trybu
            var currentTheme = AppInfo.RequestedTheme;
            var savedTheme = Preferences.Get("AppTheme", string.Empty);
            if (currentTheme.ToString() != savedTheme.ToString())
            {
                UpdateStylesBasedOnTheme();
                // Zapisz aktualny tryb do ustawień
                Preferences.Set("AppTheme", currentTheme.ToString());

                // Ponowne uruchomienie aplikacji
                MainPage = new AppShell();
            }
        }
    }
}
