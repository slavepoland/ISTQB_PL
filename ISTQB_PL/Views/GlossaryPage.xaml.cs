using ISTQB_PL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ISTQB_PL.Services;
using Xamarin.Essentials;
using MarcTron.Plugin.Controls;

namespace ISTQB_PL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GlossaryPage : ContentPage
    {
        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutReklama { get; set; }
        //private View MyAdView { get; set; }
        private MTAdView AdMobBanner { get; set; }

        SearchBar MySearchBar { get; set; }
        Switch SwitchControl {  get; set; }
        Label MyLabelText {  get; set; }
        StackLayout StackLayoutInsideScrollView { get; set; }
        CollectionView ItemsListView {  get; set; }

        GlossaryViewModel ViewModel { get; set; }
        private int FontSize { get; set; }


        public GlossaryPage(MTAdView adMobBanner)
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
                BindingContext = ViewModel = new GlossaryViewModel();
                FontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
              
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                CreateLayout();

                if (FontSize != int.Parse(Application.Current.Properties["FontSize"].ToString()))
                {
                    FontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
                    foreach (var item in ViewModel.Items)
                    {
                        item.MyFontSize = FontSize;
                    }
                }
                MySearchBar.Text = "t";
                MySearchBar.Text = string.Empty;
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

        private void CreateLayout()
        {
            MainStackLayout = new StackLayout
            {
                Style = (Style)Application.Current.Resources["StackLayoutStyle"],
                Padding = new Thickness(0)
            };

            var GridSearchBar = new Grid
            {
                BackgroundColor = Color.AliceBlue
            };

            GridSearchBar.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            GridSearchBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            GridSearchBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            GridSearchBar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            MySearchBar = new SearchBar
            {
                BackgroundColor = Color.AliceBlue,
                Placeholder = "Wyszukaj...",
                SearchCommand = ViewModel.SearchCommand,
                MinimumWidthRequest = 60
            };
            MySearchBar.SetBinding(SearchBar.TextProperty, "SearchText");
            MySearchBar.SetBinding(SearchBar.SearchCommandProperty, "SearchCommand");

            SwitchControl = new Switch
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(10),
                OnColor = Color.Green,
                ThumbColor = Color.White,
                MinimumHeightRequest = 50,
                MinimumWidthRequest = 50,
            };
            SwitchControl.BindingContext = ViewModel;
            SwitchControl.SetBinding(Switch.IsToggledProperty, new Binding("StartsWithSearchMode", mode: BindingMode.TwoWay));

            MyLabelText = new Label
            {
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.Center,
                // Dodać resztę ustawień
            };
            MyLabelText.BindingContext = ViewModel;
            MyLabelText.SetBinding(Label.TextProperty, "LabelText");

            // Dodaj kontrolki do grid
            GridSearchBar.Children.Add(MySearchBar, 0, 0);
            GridSearchBar.Children.Add(SwitchControl, 1, 0);
            GridSearchBar.Children.Add(MyLabelText, 2, 0);

            var scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Never
            };

            StackLayoutInsideScrollView = new StackLayout
            {
                Style = (Style)Application.Current.Resources["StackLayoutStyle"],
            };

            ItemsListView = new CollectionView
            {
                //ItemsSource = ViewModel.SearchItems,
                SelectionMode = SelectionMode.None
                // Dodać resztę ustawień
            };
            ItemsListView.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical);
            ItemsListView.SetBinding(CollectionView.ItemsSourceProperty, "SearchItems");
            AutomationProperties.SetName(ItemsListView, "Glosariusz ISTQB");

            ItemsListView.ItemTemplate = new DataTemplate(() =>
            {
                var stackCollectionView = new StackLayout
                {
                    
                };
                AutomationProperties.SetName(stackCollectionView, "Glosariusz ISTQB");

                var frameCollectionView = new Frame
                {
                    Style = (Style)Application.Current.Resources["FrameStyle"],
                    Margin = new Thickness(2)
                };
                AutomationProperties.SetName(frameCollectionView, "Glosariusz ISTQB");

                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

                var labelName = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    TextColor = (Color)Application.Current.Resources["JasnyTekst"],
                // Dodać resztę ustawień
                };
                labelName.SetBinding(Label.TextProperty, "Name");
                labelName.SetBinding(Label.FontSizeProperty, new Binding("MyFontSize"));
                AutomationProperties.SetName(labelName, "Definicja ze słownika ISTQB");
;
                var labelDescription = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Start,
                    TextColor = (Color)Application.Current.Resources["JasnyTekst"]
                    // Dodać resztę ustawień
                };
                labelDescription.SetBinding(Label.TextProperty, "Description");
                labelDescription.SetBinding(Label.FontSizeProperty, new Binding("MyFontSize"));
                AutomationProperties.SetName(labelDescription, "WYjaśnienie definicji ze słownika ISTQB");

                innerGrid.Children.Add(labelName, 0, 0);
                innerGrid.Children.Add(labelDescription, 1, 0);

                frameCollectionView.Content = innerGrid;
                stackCollectionView.Children.Add(frameCollectionView);

                return stackCollectionView;
            });

            StackLayoutInsideScrollView.Children.Add(ItemsListView);
            scrollView.Content = StackLayoutInsideScrollView;

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
            // Dodaj kontrolki do głównego StackLayout
            MainStackLayout.Children.Add(GridSearchBar);
            MainStackLayout.Children.Add(scrollView);
            MainStackLayout.Children.Add(StackLayoutReklama);

            // Ustaw ContentPage.Content
            Content = MainStackLayout;
        }
    }
}