using ISTQB_PL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using MarcTron.Plugin.Controls;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
//using ISTQB_PL.Services;
using Xamarin.Essentials;

namespace ISTQB_PL.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DynamicSylabusPage : ContentPage
    {
        private int myFontSize;
        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutSylabus {  get; set; }
        StackLayout StackLayoutReklama { get; set; }
        //private View MyAdView { get; set; }
        private MTAdView AdMobBanner { get; set; }

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        private SylabusWersjaViewModel ViewModel { get; set; }

        public DynamicSylabusPage(MTAdView adMobBanner)
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                AdMobBanner = adMobBanner;
                ViewModel = new SylabusWersjaViewModel();
                this.Appearing += OnPageAppearing;
            }
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var labelsInHierarchy = FindLabelInHierarchy(StackLayoutSylabus);
                    foreach (Label item in labelsInHierarchy)
                    {
                        item.FontSize = MyFontSize;
                    }
                }
                //return false aby zatrzymać timer
                return false;
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
                MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
                MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
                CreateLayout();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Sprawdź, czy bieżąca strona jest typu Shell, a jeśli tak, zamknij ją
            if (Shell.Current != null && Shell.Current.Navigation.NavigationStack.Count == 1)
            {
                Shell.Current.GoToAsync("//AboutPage");
                //Shell.Current.Navigation.PopAsync();
                return true; // Zapobiegnij standardowemu zachowaniu przycisku "Wstecz"
            }
            else
            {
                Shell.Current.GoToAsync("..");
            }
            // Standardowe zachowanie przycisku "Wstecz", jeśli nie jesteśmy w Shell
            return base.OnBackButtonPressed();
        }

        private List<Label> FindLabelInHierarchy(View view)
        {
            List<Label> labels = new List<Label>();

            if (view is Label)
            {
                labels.Add(view as Label);
            }
            else if (view is Grid)
            {
                var grid = view as Grid;
                foreach (var child in grid.Children)
                {
                    labels.AddRange(FindLabelInHierarchy(child));
                }
            }
            else if (view is Frame)
            {
                var frame = view as Frame;
                if (frame.Content is Grid)
                {
                    var contentGrid = frame.Content as Grid;
                    labels.AddRange(FindLabelInHierarchy(contentGrid));
                }
            }
            else if (view is StackLayout)
            {
                var stackLayout = view as StackLayout;
                foreach (var child in stackLayout.Children)
                {
                    labels.AddRange(FindLabelInHierarchy(child));
                }
            }
            return labels;
        }

        private void CreateLayout()
        {
            MainStackLayout = new StackLayout
            {
                Style = (Style)Application.Current.Resources["StackLayoutStyle"]
            };
            // Create a ScrollView
            var scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Default
            };

            // Create a StackLayout to hold content
            StackLayoutSylabus = new StackLayout
            {
                Padding = 5,
                BackgroundColor = MainBackgroundColor,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            Grid myGridMain = new Grid
            {
            };
            Frame myFrame = new Frame();

            var filteredItems = ViewModel.Items.Select(item => item.Wersja);

            for (int i = 0; i < filteredItems.Count(); i++) //dodanie tylu wierszy ile jest wersji Sylabusa do myGridPodrozdzial
            {
                myGridMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var sylabuswersja = ViewModel.Items.FirstOrDefault(item => item.Id == (i+1).ToString());

                myFrame = new Frame
                {
                    BackgroundColor = MainBackgroundColor,
                    BorderColor = Color.FromHex("#2196F3"),
                    ClassId = $"{sylabuswersja.Sylabus}:{sylabuswersja.Jezyk}:{sylabuswersja.Wersja}",
                    CornerRadius = 10,
                    Padding = 2,
                };

                var stackLayoutFrame = new StackLayout
                {
                    Padding = 5,
                    BackgroundColor = MainBackgroundColor
                };

                //Create a Label to put inside the Frame
                Label MainLabel = new Label
                {
                    Text = $"{sylabuswersja.Sylabus} - {sylabuswersja.Jezyk} {sylabuswersja.Wersja}",
                    FontSize = MyFontSize + 1,
                    TextColor = MainTextColor,
                    FontAttributes = FontAttributes.Bold,
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 60,
                };

                stackLayoutFrame.Children.Add(MainLabel);

                // Add a TapGestureRecognizer to the Frame
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (sender, e) =>
                {
                    var tappedFrame = (Frame)sender;
                    string frameIdentifier = tappedFrame.ClassId; // Get the unique identifier
                    HandleTap(frameIdentifier);
                };

                myFrame.GestureRecognizers.Add(tapGesture);
                // Add a TapGestureRecognizer to the Frame

                // Create a Label to put inside the Frame
                myFrame.Content = stackLayoutFrame;
                myGridMain.Children.Add(myFrame, 0, i);
            }//dodanie tylu wierszy ile jest podrozdziałów do myGridPodrozdzial

            // Add the Grid to the content of the page
            StackLayoutSylabus.Children.Add(myGridMain);
            scrollView.Content = StackLayoutSylabus;
            MainStackLayout.Children.Add(scrollView);

            // Create a StackLayout to hold ads
            StackLayoutReklama = new StackLayout
            {
                Style = (Style)Application.Current.Resources["ReklamaLayoutStyle"],
            };
            AutomationProperties.SetName(StackLayoutReklama, "Reklama");
            AutomationProperties.SetHelpText(StackLayoutReklama, "Reklama");

            //if (MyAdView == null)
            //{
            //    AdAdsService adAdsService = new AdAdsService();
            //    MyAdView = adAdsService.GetAdsGoogleView();
            //    StackLayoutReklama.Children.Add(MyAdView);
            //}

            StackLayoutReklama.Children.Add(AdMobBanner);
            MainStackLayout.Children.Add(StackLayoutReklama);
            Content = MainStackLayout;
            Title = "Sylabus";
        }

        private async void HandleTap(string sylabusVersion)
        {
            Application.Current.Properties["SylabusVersion"] = sylabusVersion;
            // Pokaż Popup z aktywatorem
            var popup = new MyPopupPage();
            await PopupNavigation.Instance.PushAsync(popup);
            //await Shell.Current.GoToAsync("/DynamicSylabusWersjaPage");
            await Navigation.PushAsync(new DynamicSylabusWersjaPage(AdMobBanner));
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAsync();
        }

    }
}