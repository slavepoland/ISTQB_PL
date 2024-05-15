using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Linq;
using System.Diagnostics;
using MarcTron.Plugin;
using MarcTron.Plugin.Controls;
using MarcTron.Plugin.Extra;
using Xamarin.Essentials;
using ISTQB_PL.ViewModels;
using System.Threading.Tasks;


namespace ISTQB_PL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        private int WrongAnswerCount {  get; set; }
        private int ExamSavedCount { get; set; }
        private int ItemsAllCount { get; set; }

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }
        private int myFontSize;

        private readonly string FileName = "answers.txt";
        private readonly string ExamFileNumber = "examnumber.txt";
        private readonly string ItemsFileNumber = "itemsnumber.txt";
        private string FilePath;

        private StackLayout MainStackLayout { get; set; }
        private StackLayout StackLayoutReklama { get; set; }
        private Grid GridButton { get; set; }
        //private View MyAdView { get ; set; }
        private MTAdView AdMobBanner { get; set; }
        private bool isInterstitialLoaded = false;

        private Button BtnItems { get; set; }
        private Button BtnExamSaved { get; set; }
        private Button BtnItemsWrongAnswer { get; set; }

        public AboutPage()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                try
                {
                    InitializeComponent();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                _ = Task.Run(LoadInterstitial);
                //zapisanie głównych ustawień aplikacji

                if (!Application.Current.Properties.ContainsKey("OdpSwitch"))
                {
                    Application.Current.Properties["OdpSwitch"] = false;
                }
                if (!Application.Current.Properties.ContainsKey("ExpSwitch"))
                {
                    Application.Current.Properties["ExpSwitch"] = false;
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
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            AdMobBanner ??= new()
            {
                IsVisible = true,
                AutoSize = true,
                AdSize = BannerSize.AnchoredAdaptive,
                AdsId = "ca-app-pub-6479256761216523/8121462788",
                //ca-app-pub-3940256099942544/6300978111   ca-app-pub-3940256099942544/9214589741
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            //AdMobBanner.LoadAd();

            AdMobBanner.AdsFailedToLoad += (sender, args) =>
            {
                if (args is MTEventArgs adError)
                {
                    string errorMessage = adError?.ErrorMessage;
                    int errorCode = (int)adError?.ErrorCode;
                    string errorDomain = adError.ErrorDomain;
                    int error1 = adError.RewardAmount;
                    string error2 = adError.RewardType;
                }
            };
            try
            {
                MyFontSize = Application.Current.Properties.ContainsKey("FontSize") ?
                int.Parse(Application.Current.Properties["FontSize"].ToString()) : 18;

                CreateLayout();
                ExistWrongAnswerAndExamSaved();
                BtnItems.Text = $"Manual tester - pytania({ItemsAllCount})";
                BtnExamSaved.Text = $"Zapisane Zestawy pytań({ExamSavedCount})";
                BtnItemsWrongAnswer.Text = $"Błędne odpowiedzi({WrongAnswerCount})";
                while (PopupNavigation.Instance.PopupStack.Count > 0)
                {
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void CreateLayout()
        {
            if (MainStackLayout == null)
            { 
                MainStackLayout = new StackLayout
                {
                    Style = (Style)Application.Current.Resources["StackLayoutStyle"]
                };
                GridButton = new Grid
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent
                };

                GridButton.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GridButton.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GridButton.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GridButton.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); //{ Height = GridLength.Auto}

                GridButton.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                GridButton.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                //Create a StackLayout to hold ads
                StackLayoutReklama = new StackLayout
                {
                    Style = (Style)Application.Current.Resources["ReklamaLayoutStyle"],
                };
                Grid.SetRow(StackLayoutReklama, 3);
                Grid.SetColumn(StackLayoutReklama, 0);
                Grid.SetColumnSpan(StackLayoutReklama, 2);
                GridButton.Children.Add(StackLayoutReklama);

                Button BtnSylabus = new Button
                {
                    Text = "Sylabus",
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                };
                BtnSylabus.Clicked += Button_Clicked_Sylabus;
                Grid.SetRow(BtnSylabus, 0);
                Grid.SetColumn(BtnSylabus, 0);

                Button BtnGlossary = new Button
                {
                    Text = "Glosariusz",
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                };
                BtnGlossary.Clicked += Button_Clicked_Glossary; 
                Grid.SetRow(BtnGlossary, 0);
                Grid.SetColumn(BtnGlossary, 1);

                Button BtnExam = new Button
                {
                    Text = "Zestawy pytań - egzamin",
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                };
                BtnExam.Clicked += Button_Clicked_Exam;
                Grid.SetRow(BtnExam, 1);
                Grid.SetColumn(BtnExam, 0);

                BtnItems = new Button
                {
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                };
                BtnItems.Clicked += Button_Clicked_Items;
                Grid.SetRow(BtnItems, 1);
                Grid.SetColumn(BtnItems, 1);

                BtnExamSaved = new Button
                {
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                    IsEnabled = false,
                };
                BtnExamSaved.Clicked += Button_Clicked_ExamSaved;
                Grid.SetRow(BtnExamSaved, 2);
                Grid.SetColumn(BtnExamSaved, 0);

                BtnItemsWrongAnswer = new Button
                {
                    Margin = new Thickness(5),
                    BackgroundColor = (Color)Application.Current.Resources["Primary"],
                    TextColor = Color.White,
                    FontSize = MyFontSize,
                    IsEnabled = false,
                };
                BtnItemsWrongAnswer.Clicked += Button_Clicked_ItemsWrong;
                Grid.SetRow(BtnItemsWrongAnswer, 2);
                Grid.SetColumn(BtnItemsWrongAnswer, 1);

                GridButton.Children.Add(BtnSylabus);
                GridButton.Children.Add(BtnGlossary);
                GridButton.Children.Add(BtnExam);
                GridButton.Children.Add(BtnItems);
                GridButton.Children.Add(BtnExamSaved);
                GridButton.Children.Add(BtnItemsWrongAnswer);

                MainStackLayout.Children.Add(GridButton);
                Content = MainStackLayout;
            }
            do
            {
                try
                {
                    if (!(StackLayoutReklama.Children.Count == 1 && StackLayoutReklama.Children[0] is MTAdView))
                    {
                        StackLayoutReklama.Children.Add(AdMobBanner);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            } while (AdMobBanner == null);
        }

        private async void MyDisplayAlert()
        {
            await DisplayAlert("Brak dostępu do Internetu", "Opcja niedostępna.", "OK");
        }

        private void LoadInterstitial()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CrossMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
                    {
                        //CrossMTAdmob.Current.ShowInterstitial();
                        isInterstitialLoaded = true;
                    };
                    CrossMTAdmob.Current.OnInterstitialFailedToLoad += (s, args) =>
                    {
                        if (args is MTEventArgs adError)
                        {
                            string errorMessage = adError?.ErrorMessage;
                            int errorCode = (int)adError?.ErrorCode;
                            string errorDomain = adError.ErrorDomain;
                            int error1 = adError.RewardAmount;
                            string error2 = adError.RewardType;
                        }
                    };
                    CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-6479256761216523/7630984525");
                    // reklama testowa ca-app-pub-3940256099942544/1033173712 ca-app-pub-3940256099942544/8691691433
                    //moja reklama pełnoekranowa 
                });
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        private void ShowInterstitialIfLoaded()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (isInterstitialLoaded)
                    {
                        CrossMTAdmob.Current.ShowInterstitial();
                        isInterstitialLoaded = false; // Ustawiamy na false, aby można było ponownie załadować reklamę
                        _ = Task.Run(LoadInterstitial); // Wczytujemy kolejną reklamę po wyświetleniu poprzedniej
                    }
                });
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); } 
        }

        private async void Button_Clicked_Sylabus(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //reklamy
                _ = Task.Run(ShowInterstitialIfLoaded);

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                
                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //await Shell.Current.GoToAsync("//DynamicSylabusPage");
                            await Navigation.PushAsync(new DynamicSylabusPage(AdMobBanner));
                        });
                    });
                }
                catch(Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }

                while (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }

        }

        private async void Button_Clicked_Exam(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //popup z wyborem wersji Sylabusa do egzaminu
                var popupExam = new ExamPopupPage();
                var viewModel = new ExamPopupViewModel();
                popupExam.BindingContext = viewModel;
                await PopupNavigation.Instance.PushAsync(popupExam);
                //czekaj aż użytkownik potwierdzi zamknięcie wyboru
                await viewModel.WaitForPopupCloseAsync();

                //await WaitForInterstitialLoadedAsync();
                _ = Task.Run(ShowInterstitialIfLoaded);

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();

                await PopupNavigation.Instance.PushAsync(popup);

                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PushAsync(new ExamMainPage());
                        });
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }

                if (PopupNavigation.Instance.PopupStack.Count > 0)
                   await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }
        }

        private async void Button_Clicked_Items(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //reklamy
                _ = Task.Run(ShowInterstitialIfLoaded);
                //testowa reklama pełnoekarnowa   ca-app-pub-3940256099942544/3419835294 ca-app-pub-3940256099942544/1033173712

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                                
                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PushAsync(new ItemsPage());
                        });
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }

                
                while (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }
        }

        private async void Button_Clicked_Glossary(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //reklamy
                _ = Task.Run(ShowInterstitialIfLoaded);

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                
                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //await Shell.Current.GoToAsync("//GlossaryPage");
                            await Navigation.PushAsync(new GlossaryPage(AdMobBanner));
                        });
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }
                
                while (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }
        }

        private async void Button_Clicked_ExamSaved(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //reklamy
                _ = Task.Run(ShowInterstitialIfLoaded);

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                
                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PushAsync(new ExamSavedPage(AdMobBanner));
                        });
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }

                while (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }
        }

        private async void Button_Clicked_ItemsWrong(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //reklamy
                _ = Task.Run(ShowInterstitialIfLoaded);

                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);

                try
                {
                    _ = Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PushAsync(new ItemsPageWrongAnswer());
                        });
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert(title: Title, ex.Message, "Ok");
                }
                
                while (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                MyDisplayAlert();
            }
        }

        private void ExistWrongAnswerAndExamSaved()
        {
            //check Wrong Answers, if exist, BtnItemsWrongAnswer.IsEnabled = true;
            //Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected 
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), FileName);
            WrongAnswerCount = 0;
            if (File.Exists(FilePath))
            {
                string[] readContent = ReadAndSplitFile();       
                if (readContent.Count() > 0) //&& readContent[0] != ""
                {
                    for (int i = 0; i < readContent.Count(); i++)
                    {
                        string[] answer = readContent[i].Split(':');
                        if (answer[4] == "nie")
                        {
                            WrongAnswerCount++;
                        }
                    }
                    if(WrongAnswerCount > 0)
                    {
                        BtnItemsWrongAnswer.IsEnabled = true;
                    }
                }
            }

            //check Exam Saved, if exist, BtnExamSaved.IsEnabled = true;
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), ExamFileNumber);
            if (File.Exists(FilePath))
            {

                BtnExamSaved.IsEnabled = true;
                ExamSavedCount = int.Parse(File.ReadAllText(FilePath));
            }
            else
            {
                ExamSavedCount = 0;
            }

            //check All Items Count
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), ItemsFileNumber);
            if (File.Exists(FilePath))
            {
                ItemsAllCount = int.Parse(File.ReadAllText(FilePath));
            }
            else
            {
                ItemsAllCount = 500;
            }
        }

        private string[] ReadAndSplitFile()
        {
            try
            {
                // Odczyt z pliku
                string fileContent = File.ReadAllText(FilePath);

                // Podział na string[] na podstawie znaków nowej linii
                string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                return lines;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Błąd podczas odczytu pliku(ReadAndSplitFile):", ex.Message);
                return new string[0]; // Zwrócenie pustej tablicy w przypadku błędu
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            // Pokaż Popup z aktywatorem
            var popup = new MyPopupPage();
            await PopupNavigation.Instance.PushAsync(popup);
            await Navigation.PushAsync(new SettingsPage());
            while (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAsync();
        }

    }
}