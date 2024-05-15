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
	public partial class SylabusArticlePage : ContentPage
	{
        private SylabusViewModel ViewModel { get; set; }

        StackLayout MainStackLayout { get; set; }
        StackLayout StackLayoutSylabus { get; set; }
        StackLayout StackLayoutReklama { get; set; }
        //private View MyAdView { get; set; }
        private MTAdView AdMobBanner { get; set; }

        private int myFontSize;
        private string podrozdzial;

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }
        string Podrozdzial
        {
            get => podrozdzial;
            set => podrozdzial = value;
        }

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public SylabusArticlePage (string podrozdzial, SylabusViewModel viewModel, MTAdView adMobBanner)
		{
			InitializeComponent ();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
                AdMobBanner = adMobBanner;
                Podrozdzial = podrozdzial ?? string.Empty;
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
            ScrollView scrollView = new ScrollView
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Default
            };
            // Create a StackLayout to hold content
            StackLayoutSylabus = new StackLayout
            {
                Style = (Style)Application.Current.Resources["StackLayoutStyle"],
            };
            // Create a Frame to hold content
            Frame myFrame = new Frame
            {
                BackgroundColor = MainBackgroundColor,
                BorderColor = Color.FromHex("#2196F3"),
                CornerRadius = 10,
                Padding = 2,
            };
            // Create a StackLayout to hold Label in the Frame
            StackLayout stackLayoutFrame = new StackLayout
            {
                Padding = 2,
                BackgroundColor = MainBackgroundColor,
            };

            var podrozdzialItems = ViewModel.Items.Where(item => item.Podrozdzial == Podrozdzial);

            int podrozdzial = int.Parse(podrozdzialItems.LastOrDefault().Podpodrozdzial.Split('.').LastOrDefault());

            string myFLlabelText = "";
            foreach (var item in podrozdzialItems)
            {
                if (item.Umiejetnosci != "null")
                    myFLlabelText += $"{item.Umiejetnosci}\n";
            }
            if(myFLlabelText != "")
            {
                Label myFLlabel = new Label
                {
                    Text = myFLlabelText[..(myFLlabelText.Count() - 2)],
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = MyFontSize + 1,
                    TextColor = MainTextColor,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                // Add the Label to the StackLayout
                stackLayoutFrame.Children.Add(myFLlabel);
                // Add Label to put inside the Frame
                myFrame.Content = stackLayoutFrame;
                StackLayoutSylabus.Children.Add(myFrame);
            }

            foreach (var item in podrozdzialItems)
            {
                myFrame = new Frame
                {
                    BackgroundColor = MainBackgroundColor,
                    BorderColor = Color.FromHex("#2196F3"),
                    CornerRadius = 10,
                    Padding = 2,
                };
                stackLayoutFrame = new StackLayout
                {
                    Padding = 2,
                    BackgroundColor = MainBackgroundColor,
                };

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
                };
                Label myLabelContent = new Label
                {
                    Text = item.Content,
                    VerticalTextAlignment = TextAlignment.Start,
                    FontSize = MyFontSize,
                    TextColor = MainTextColor,
                    Padding = 2,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                // Add the Label to the StackLayout
                stackLayoutFrame.Children.Add(myLabel);
                stackLayoutFrame.Children.Add(myLabelContent);
                // Add Label to put inside the Frame
                myFrame.Content = stackLayoutFrame;
                StackLayoutSylabus.Children.Add(myFrame);
            }
               
            // Add the StackLayout to the content of the page
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
            Title = podrozdzialItems.FirstOrDefault().Podrozdzial_description;
        }
    }
}