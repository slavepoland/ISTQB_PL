using ISTQB_PL.Services;
using ISTQB_PL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Essentials;
using MarcTron.Plugin.Controls;


namespace ISTQB_PL.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DynamicSylabusWersjaPage : ContentPage
    {
        private int myFontSize;
        private string SylabusVersion { get; set; }

        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutSylabus { get; set; }
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

        private SylabusViewModel ViewModel { get; set; }
        private SylabusViewModel ViewModelVersion { get; set; }

        public DynamicSylabusWersjaPage(MTAdView adMobBanner) //string sylabusVersion
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
                ViewModel = new SylabusViewModel(true);
                ViewModelVersion = new SylabusViewModel(true);
                this.Appearing += OnPageAppearing;
            }   
            
            SylabusVersion = Application.Current.Properties["SylabusVersion"].ToString();
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    // Sprawdź, czy bieżąca strona jest typu Shell, a jeśli tak, zamknij ją
        //    if (Shell.Current != null && Shell.Current.Navigation.NavigationStack.Count == 1)
        //    {
        //        Shell.Current.GoToAsync("//DynamicSylabusPage");
        //        //Shell.Current.Navigation.PopAsync();
        //        return true; // Zapobiegnij standardowemu zachowaniu przycisku "Wstecz"
        //    }
        //    else
        //    {
        //        Shell.Current.GoToAsync("..");
        //    }
        //    // Standardowe zachowanie przycisku "Wstecz", jeśli nie jesteśmy w Shell
        //    return base.OnBackButtonPressed();
        //}

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
                BackgroundColor = MainBackgroundColor,
                Style = (Style)Application.Current.Resources["StackLayoutStyle"],
            };
            // Create a ScrollView
            var scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Default
            };

            // Create a StackLayout to hold content
            StackLayoutSylabus = new StackLayout
            {
                Style = (Style)Application.Current.Resources["StackLayoutStyle"],
            };
            // Pobierz styl z zasobów aplikacji
            Style stackLayoutStyle = (Style)Application.Current.Resources["StackLayoutStyle"];

            // Przypisz styl do StackLayout
            StackLayoutSylabus.Style = stackLayoutStyle;

            Grid myGridMain = new Grid
            {
            };
            Frame myFrame = new Frame();

            var version = SylabusVersion.Split(':');
            //var viewModelVersion = ViewModel.Items.Where(x => x.Wersja == version[2].ToString()).Select(x => x);
            ViewModelVersion.Items.Clear();

            foreach (var viewModel in ViewModel.Items)
            {
                if(viewModel.Wersja  == version[2].ToString())
                    ViewModelVersion.Items.Add(viewModel);
            }
            var rozdzial = ViewModelVersion.Items.Select(item => item.Rozdzial).Distinct();

            var rozdzial_description = ViewModelVersion.Items.Select(item => item.Rozdzial_description).Distinct().ToArray();


            for (int i = 0; i < rozdzial.Count(); i++) //dodanie tylu wierszy ile jest wersji Sylabusa do myGridPodrozdzial
            {
                myGridMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                myFrame = new Frame
                {
                    BackgroundColor = MainBackgroundColor,
                    BorderColor = Color.FromHex("#2196F3"),
                    ClassId = $"{rozdzial.FirstOrDefault(x => x == (i+1).ToString())}",
                    CornerRadius = 10,
                    Padding = 2,
                };

                var stackLayoutFrame = new StackLayout
                {
                    Padding = 5,
                    BackgroundColor = MainBackgroundColor
                };

                //Create a Label to put inside the Frame
                Label MainLabelRozdzial = new Label
                {
                    Text = $"Rozdział {rozdzial.FirstOrDefault(x => x == (i+1).ToString())}",
                    FontSize = MyFontSize + 1,
                    TextColor = MainTextColor,
                    FontAttributes = FontAttributes.Bold,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                Label MainLabelDescription = new Label
                {
                    Text = $"{rozdzial_description[i]}",
                    FontSize = MyFontSize,
                    TextColor = MainTextColor,
                    FontAttributes = FontAttributes.Bold,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };

                stackLayoutFrame.Children.Add(MainLabelRozdzial);
                stackLayoutFrame.Children.Add(MainLabelDescription);

                // Add a TapGestureRecognizer to the Frame
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (sender, e) =>
                {
                    var tappedFrame = (Frame)sender;
                    string frameIdentifier = tappedFrame.ClassId; // Get the unique identifier
                    OnFrameTapped(sender, e, frameIdentifier);
                };

                myFrame.GestureRecognizers.Add(tapGesture);
                // Add a TapGestureRecognizer to the Frame

                // Create a Label to put inside the Frame
                myFrame.Content = stackLayoutFrame;
                myGridMain.Children.Add(myFrame, 0, i);
            }//dodanie tylu wierszy ile jest podrozdziałów do myGridPodrozdzial

            ////dodanie nowego wiersza w Gridzie, Label + Image 
            //myGridMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //myGridMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //// dodanie z Label z tekstem
            //Label MainLabelTekst = new Label
            //{
            //    Text = "Na podstawie:",
            //    FontSize = MyFontSize,
            //    TextColor = MainTextColor,
            //    Padding = 2,
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //};
            //myGridMain.Children.Add(MainLabelTekst, 0, rozdzial.Count());
            ////dodanie obrazka z ISTQB, Utwórz obiekt Image
            //var cachedImage = new CachedImage
            //{
            //    Source = new UriImageSource
            //    {
            //        Uri = new Uri("http://arka1mtb.cba.pl/istqb_image/logoistqb.jpg"),
            //        CachingEnabled = true,
            //        CacheValidity = new TimeSpan(7, 0, 0, 0) // Długość ważności w cache (7 dni w tym przypadku)
            //    },
            //    LoadingPlaceholder = "loading_placeholder.png", // Obrazek zastępczy podczas ładowania
            //    ErrorPlaceholder = "error_placeholder.png", // Obrazek zastępczy w przypadku błędu ładowania
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    WidthRequest = 200,
            //    HeightRequest = 160,
            //    DownsampleToViewSize = true,
            //    InputTransparent = true,

            //};
            //myGridMain.Children.Add(cachedImage, 0, rozdzial.Count() + 1);

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
            //}
            StackLayoutReklama.Children.Add(AdMobBanner);
            MainStackLayout.Children.Add(StackLayoutReklama);

            Content = MainStackLayout;
            Title = $"{SylabusVersion.Split(':')[0]} - {SylabusVersion.Split(':')[1]} {SylabusVersion.Split(':')[2]}";
        }

        private async void OnFrameTapped(object sender, EventArgs e, string frameIdentifier)
        {
            if (sender is Frame) // tappedFrame
            {
                //string nrRozdzial = (string)((TapGestureRecognizer)tappedFrame.GestureRecognizers[0])
                //    .CommandParameter;
                await Navigation.PushAsync(new SylabusChapterPage(ViewModelVersion, frameIdentifier, AdMobBanner));
            }
        }
    }
}