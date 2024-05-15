using ISTQB_PL.Services;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Essentials;
using System.Diagnostics;
using System.IO;
using MarcTron.Plugin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExamSavedPage : ContentPage
	{
        private readonly string ExamFileName = "exam.txt";
        private readonly string ExamFileNumber = "examnumber.txt";
        private string FilePath;

        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutExamNumber { get; set; }
        StackLayout StackLayoutReklama { get; set; }
        //private View MyAdView { get; set; }
        private MTAdView AdMobBanner { get; set; }

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        private int myFontSize;
        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }

        public ExamSavedPage(MTAdView adMobBanner)
        {
			InitializeComponent ();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                AdMobBanner = adMobBanner;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Sprawdź, czy bieżąca strona jest typu Shell, a jeśli tak, zamknij ją
            if (Shell.Current != null && Shell.Current.Navigation.NavigationStack.Count == 1)
            {
                Shell.Current.GoToAsync("//AboutPage");
                return true; // Zapobiegnij standardowemu zachowaniu przycisku "Wstecz"
            }
            else
            {
                Shell.Current.GoToAsync("..");
            }
            // Standardowe zachowanie przycisku "Wstecz", jeśli nie jesteśmy w Shell
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                CreateLayout();
            }
        }

        private void CreateLayout()
        {
            //pobranie  numerów egzaminu 
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), ExamFileNumber);
            string examnumber = File.ReadAllText(FilePath);

            MainStackLayout = new StackLayout
            {
                BackgroundColor = MainBackgroundColor,
            };
            // Create a ScrollView
            ScrollView scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Default
            };

            // Pobierz styl z zasobów aplikacji
            Style mainStackLayoutStyle = (Style)Application.Current.Resources["StackLayoutStyle"];

            // Przypisz styl do StackLayout
            MainStackLayout.Style = mainStackLayoutStyle;

            // Create a StackLayout to hold content
            StackLayoutExamNumber = new StackLayout
            {
                Padding = 5,
                BackgroundColor = MainBackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            // Przypisz styl do StackLayout
            StackLayoutExamNumber.Style = (Style)Application.Current.Resources["StackLayoutStyle"];

            for(int i = 0; i < int.Parse(examnumber); i++)
            {
                Frame MyFrame = new Frame
                {
                    BackgroundColor = MainBackgroundColor,
                    BorderColor = Color.FromHex("#2196F3"),
                    ClassId = $"{i + 1}",
                    CornerRadius = 10,
                    Padding = 2,
                };
                // Przypisz styl do Frame
                MyFrame.Style = (Style)Application.Current.Resources["FrameStyle"];

                //Create a Label to put inside the Frame
                Label MainLabel = new Label
                {
                    Text = $"Egzamin nr {i + 1}",
                    FontSize = MyFontSize + 1,
                    TextColor = MainTextColor,
                    FontAttributes = FontAttributes.Bold,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 60,
                };

                // Add a TapGestureRecognizer to the Frame
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (sender, e) =>
                {
                    var tappedFrame = (Frame)sender;
                    string frameIdentifier = tappedFrame.ClassId; // Get the unique identifier
                    HandleTap(frameIdentifier);
                };

                MyFrame.GestureRecognizers.Add(tapGesture);
                // Add a TapGestureRecognizer to the Frame

                MyFrame.Content = MainLabel;

                StackLayoutExamNumber.Children.Add(MyFrame);
            }
            scrollView.Content = StackLayoutExamNumber;
            MainStackLayout.Children.Add(scrollView);
            // Create a StackLayout to hold ads
            StackLayoutReklama = new StackLayout
            {
                Style = (Style)Application.Current.Resources["ReklamaLayoutStyle"],
            };
            //AdAdsService adAdsService = new AdAdsService();
            //MyAdView = adAdsService.GetAdsGoogleView();
            StackLayoutReklama.Children.Add(AdMobBanner);

            MainStackLayout.Children.Add(StackLayoutReklama);

            Content = MainStackLayout;
            Title = "Zapisane egzaminy";
        }

        private async void HandleTap(string examnumber)
        {
            // Pokaż Popup z aktywatorem
            var popup = new MyPopupPage();
            await PopupNavigation.Instance.PushAsync(popup);
            //await Shell.Current.GoToAsync("/DynamicSylabusWersjaPage");
            await Navigation.PushAsync(new ExamSavedDetailPage(examnumber));
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAsync();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Uwaga!", "Czy na pewno chcesz usunąć wszyskie zapisane egzaminy?", "Tak", "Nie");
            if (result)
            {
                try
                {
                    FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                    LocalApplicationData), ExamFileNumber);
                    File.Delete(FilePath);
                    FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                    LocalApplicationData), ExamFileName);
                    File.Delete(FilePath);

                    await Shell.Current.GoToAsync("//AboutPage");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Nie udało się usunąć pliku z egzaminem.", ex.Message);
                }
            }
        }
    }
}