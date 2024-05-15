using ISTQB_PL.Services;
using ISTQB_PL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using MarcTron.Plugin.Controls;

namespace ISTQB_PL.Views
{
	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SylabusChapterPage : ContentPage
	{
        private int myFontSize;
		private string fundationChapter;

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }
        string FundationChapter 
		{ get => fundationChapter;
		  set => fundationChapter = value; 
		}

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        private SylabusViewModel ViewModel { get; set; }
        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutSylabus { get; set; }
        StackLayout StackLayoutReklama { get; set; }
        //private View MyAdView { get; set; }
        private MTAdView AdMobBanner { get; set; }

        public SylabusChapterPage(SylabusViewModel viewModel, string nrRozdzial, MTAdView adMobBanner)
		{
			InitializeComponent();
            MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                AdMobBanner = adMobBanner;
                FundationChapter = nrRozdzial;
                ViewModel = viewModel;
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
            MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
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

            Grid myGridMain = new Grid
            {
            };
            
            var filteredItems = ViewModel.Items.Where(item => item.Rozdzial == FundationChapter);

            int podrozdzial = int.Parse(filteredItems.LastOrDefault().Podrozdzial.Split('.').LastOrDefault());

            for (int i = 0; i < podrozdzial; i++) //dodanie tylu wierszy ile jest podrozdziałów do myGridPodrozdzial
            {
                myGridMain.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var podrozdzialItems = ViewModel.Items.Where(item => item.Podrozdzial == $".{FundationChapter}.{i + 1}");

                int podpodrozdzial = int.Parse(podrozdzialItems.LastOrDefault().Podpodrozdzial.Split('.').LastOrDefault());

                //string test = $"Frame{i+1}:{podrozdzialItems.FirstOrDefault().Podrozdzial}";
                Frame myFrame = new Frame
                {
                    BackgroundColor = MainBackgroundColor,
                    BorderColor = Color.FromHex("#2196F3"),
                    ClassId = $"{podrozdzialItems.FirstOrDefault().Podrozdzial}",
                    CornerRadius = 10,
                    Padding = 2,
                };

                var stackLayoutFrame = new StackLayout
                {
                    Padding = 5,
                    BackgroundColor = MainBackgroundColor
                };

                // Create a Label to put inside the Frame
                    Label myMainLabel = new Label
                    {
                        Text = $"{podrozdzialItems.FirstOrDefault().Podrozdzial[1..]}: " +
                            $"{podrozdzialItems.FirstOrDefault().Podrozdzial_description}",
                        //VerticalTextAlignment = TextAlignment.Start,
                        FontSize = MyFontSize+1,
                        TextColor = MainTextColor,
                        FontAttributes = FontAttributes.Bold,
                        Padding= 2,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        TextType = TextType.Html,
                    };
                // Add the Label to the StackLayout
                stackLayoutFrame.Children.Add(myMainLabel);
                int k = 1;
                    foreach (var item in podrozdzialItems)
                    {
                        string myLabelText;
                        // Create a Label to put inside the Frame
                        if (item.Podpodrozdzial_description == "Opis.")
                        {
                            myLabelText = $"• {item.Podpodrozdzial_description}";
                        }
                        else
                        {
                            myLabelText = $"• {item.Podpodrozdzial[1..]} {item.Podpodrozdzial_description}";
                        }
                        Label myLabel = new Label
                        {
                            Text = myLabelText,
                            VerticalTextAlignment = TextAlignment.Start,
                            FontSize = MyFontSize,
                            TextColor = MainTextColor,
                            Padding = 2,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            TextType = TextType.Html,
                        };

                    // Add the Label to the StackLayout
                    stackLayoutFrame.Children.Add( myLabel );
                    k++;
                    }

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
                //Padding = 5,
                //BackgroundColor = MainBackgroundColor,
                //VerticalOptions = LayoutOptions.EndAndExpand,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
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
            Title = filteredItems.FirstOrDefault().Rozdzial_description;
        }

        private async void HandleTap(string podrozdzial)
        {
            // Handle the tap event based on the unique identifier
            await Navigation.PushAsync(new SylabusArticlePage(podrozdzial, ViewModel, AdMobBanner));
        }

    }
}