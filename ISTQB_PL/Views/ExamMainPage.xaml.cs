using ISTQB_PL.Models;
using ISTQB_PL.ViewModels;
using System.Timers;
using System;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using ISTQB_PL.Services;
using System.Threading.Tasks;
using System.IO;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;


namespace ISTQB_PL.Views
{
    public partial class ExamMainPage : ContentPage
    {
        private readonly int CountDownTime = 3600; //3600
        private bool koniecTestu = false;
        private readonly string ExamFileName = "exam.txt";
        private readonly string ExamFileNumber = "examnumber.txt";
        private string FilePath;

        private string zoomedImageQuestions;
        private string zoomedImageExplanation;

        private int ExamNumber {  get; set; }

        private ExamViewModel ViewModel { get; set; }
        private SelectionChangedEventArgs MySelectionChangedEventArgs {  get; set; }
        private CountdownTimer CountdownTimerProperty { get; set; }

        private int MyFontSize { get; set; }
        private bool OdpSwitch { get; set; }
        private bool ExpSwitch { get; set; }

        MyRadioButton NewRadioA { get; set; }
        MyRadioButton NewRadioB { get; set; }
        MyRadioButton NewRadioC { get; set; }
        MyRadioButton NewRadioD { get; set; }
        

        private string CurrentSelectionItemsListView = null;
        private int countClickedAnswer = 0;

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public ExamMainPage()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                BindingContext = ViewModel = new ExamViewModel("exam", string.Empty);   //"ISTQB - przykładowy egzamin"

                CountdownTimerProperty = new CountdownTimer(CountDownTime); // Set the countdown time in seconds
                CountdownTimerProperty.CountdownTick += OnCountdownTick;
                this.Appearing += OnPageAppearing;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Pauzuj odliczanie przy zniknięciu strony
            CountdownTimerProperty.Pause();
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    // Umieść tutaj kod do przewinięcia CollectionView do środka
                    foreach (var item in ViewModel.ItemsToViewModel)
                    {
                        if (!item.IsSelected)
                        {
                            ItemsListView.SelectedItem = item;

                            var selectedViewModelItem = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView);
                            if (selectedViewModelItem != null)
                            {
                                ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.LastOrDefault(), position: ScrollToPosition.Center, animate: false);
                                ItemsListView.ScrollTo(ViewModel.ItemsToViewModel[25], position: ScrollToPosition.Center, animate: false);
                                ItemsListView.ScrollTo(selectedViewModelItem, position: ScrollToPosition.Center, animate: false);
                            }
                            break;
                        }
                    }
                }
                // Zwróć true, aby zatrzymać timer po jednym wykonaniu
                return false;
            });
        }

        protected override void OnAppearing()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                base.OnAppearing();
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
                    MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
                    foreach (var item in ViewModel.ItemsToViewModel)
                    {
                        item.ItemMainTextColor = MainTextColor;
                    }
                    ViewModel.OnAppearing();

                    CountdownTimerProperty.Start();
                    MyFontSize = Application.Current.Properties.ContainsKey("FontSize") ?
                        int.Parse(Application.Current.Properties["FontSize"].ToString()) : 18;

                    OdpSwitch = Application.Current.Properties.ContainsKey("OdpSwitch")
                        && (bool)Application.Current.Properties["OdpSwitch"];

                    ExpSwitch = Application.Current.Properties.ContainsKey("ExpSwitch")
                        && (bool)Application.Current.Properties["ExpSwitch"];

                    MyLabel.FontSize = MyFontSize;
                    MyContentLabel.FontSize = MyFontSize;
                    LabelExplanation.FontSize = MyFontSize;
                    CountdownLabel.FontSize = MyFontSize;
                    CountQuestions.FontSize = MyFontSize;

                    // Refresh the selected item in the CollectionView
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
                    ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.LastOrDefault(), position: ScrollToPosition.Center, animate: false);
                    ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.FirstOrDefault(), position: ScrollToPosition.Center, animate: false);
                }
                // Zwróć true, aby zatrzymać timer po jednym wykonaniu
                return false;
            });     
        }

        protected override bool OnBackButtonPressed()
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                PopupNavigation.Instance.PopAsync();
            }
            _ = ViewModel.MainExamGoBackCommand();
            return true;
        }

        private void RadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(e.Value != false)
            {
                GridDefaultRadioButtonBorderColor(); //defaultowe ustawienia radiobuttonów

                MyRadioButton radioButton = sender as MyRadioButton;

                Item currentItem = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView);
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).IsSelected = true;
                if (CurrentSelectionItemsListView == currentItem.Id)
                {
                    currentItem.Answer_user = radioButton.Value.ToString(); //added user_answer to binding
                    currentItem.RadioBtnSelected = radioButton.Value.ToString();  // which radiobutton was clicked added to binding
                    if (currentItem.Answer_clicked == false) //if aswer was click, don't incrase countClickedAnswer
                    {
                        currentItem.Answer_clicked = true;
                        countClickedAnswer++;
                    }
                    currentItem.IsSelected = true;
                    CountQuestions.Text = countClickedAnswer.ToString() + "/40";

                    if (!OdpSwitch)
                    { //dodanie info o tym, że odpowiedź już byłą udzielona, żeby zablokować AnswerGrid
                        currentItem.Answer_checked = true;
                        GridAnswer.IsEnabled = false;
                    }

                    //sprawdzenie odpowiedzi, na zielone jeżeli prawdidłowa, błędna odpowiedź -zaznaczenie dwóch RB, prawdiiłowa odpowiedź na zielone, błędna na czerwono 
                    if (currentItem.Answer_user.ToLower() == currentItem.Answer_right.ToLower())
                    {
                        AnswerRight(currentItem.Id);
                    }
                    else
                    {
                        AnswerWrong(currentItem.Id);
                    }
                }
                ChangeFrameBackgroundColor(false);
            } 
        }

        private void UpdateCurrentItemLabel() //SelectionChangedEventArgs e
        {
            MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
            LabelExplanation.Text = "";
            PictureExplanation.IsVisible = false;
            FrameExplanation.IsVisible = false;

            Item currentItem;
            if (MySelectionChangedEventArgs != null)
            {
                currentItem = MySelectionChangedEventArgs.CurrentSelection[0] as Item;
                CurrentSelectionItemsListView = currentItem.Id;
            }
            else
            {
                currentItem = ViewModel.ItemsToViewModel[0];
                CurrentSelectionItemsListView = ViewModel.ItemsToViewModel[0].Id;
            }

            if (currentItem != null)
            {
                if (currentItem.Str_Picture != "null") //sprawdź czy nalezy wyświetlić zdjęcie
                {
                    FFimageSource.IsVisible = true;
                    FFimageSource.Source = currentItem.Str_Picture;
                    zoomedImageQuestions = currentItem.Str_Picture;
                }
                else
                {
                    FFimageSource.IsVisible = false;
                }
                if (currentItem.Exp_Picture != "null") //sprawdź czy należy wyświetlić zdjęcie w wyjaśnieniu
                {
                    PictureExplanation.Source = currentItem.Exp_Picture;
                    zoomedImageExplanation = currentItem.Exp_Picture;
                }

                //dodanie contentu - z numerem pytania oraz danymi do radiobuttonów 
                MyLabel.Text = $"Pytanie: {currentItem.Id_Exam}, (Sylabus: {currentItem.Wersja_Sylabus[..3]}, Rozdział: {currentItem.Rozdzial})"; //, Nr pytania: {currentItem.Id}
                MyLabel.FontSize = MyFontSize;
                MyContentLabel.Text = currentItem.MyContent;
                MyContentLabel.FontSize = MyFontSize;
                //drawable new radiobutton, necessary to fit all text in radiobutton content
                GridAnswer.Children.Clear();

                NewRadioA = new MyRadioButton
                {
                    Content = currentItem.Answer_a,
                    BackgroundColor = MainBackgroundColor,
                    BorderWidth = 4,
                    BorderColor = Color.FromHex("#2196F3"),
                    TextColor = MainTextColor,
                    Padding = 5,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 10,
                    Value = "Answer_a",
                    MinimumHeightRequest = 48,
                    FontSize = MyFontSize
                };
                NewRadioB = new MyRadioButton
                {
                    Content = currentItem.Answer_b,
                    BackgroundColor = MainBackgroundColor,
                    BorderWidth = 4,
                    BorderColor = Color.FromHex("#2196F3"),
                    TextColor = MainTextColor,
                    Padding = 5,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 10,
                    Value = "Answer_b",
                    MinimumHeightRequest = 48,
                    FontSize = MyFontSize
                };
                NewRadioC = new MyRadioButton
                {
                    Content = currentItem.Answer_c,
                    BackgroundColor = MainBackgroundColor,
                    BorderWidth = 4,
                    BorderColor = Color.FromHex("#2196F3"),
                    TextColor = MainTextColor,
                    Padding = 5,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 10,
                    Value = "Answer_c",
                    MinimumHeightRequest = 48,
                    FontSize = MyFontSize
                };
                NewRadioD = new MyRadioButton
                {
                    Content = currentItem.Answer_d,
                    BackgroundColor = MainBackgroundColor,
                    BorderWidth = 4,
                    BorderColor = Color.FromHex("#2196F3"),
                    TextColor = MainTextColor,
                    Padding = 5,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    CornerRadius = 10,
                    Value = "Answer_d",
                    MinimumHeightRequest = 48,
                    FontSize = MyFontSize,
                };

                GridAnswer.Children.Add(NewRadioA, 0, 0);
                GridAnswer.Children.Add(NewRadioB, 0, 1);
                GridAnswer.Children.Add(NewRadioC, 0, 2);
                GridAnswer.Children.Add(NewRadioD, 0, 3);

                foreach (var grid in GridAnswer.Children)//dodanie obsługi Swipe
                {
                    if (grid is MyRadioButton radioButton)
                    {
                        var swipeLeftGesture = new SwipeGestureRecognizer
                        {
                            Direction = SwipeDirection.Left
                        };
                        var swipeRightGesture = new SwipeGestureRecognizer
                        {
                            Direction = SwipeDirection.Right
                        };
                        swipeLeftGesture.Swiped += OnSwiped;
                        swipeRightGesture.Swiped += OnSwiped;
                        radioButton.GestureRecognizers.Add(swipeLeftGesture);
                        radioButton.GestureRecognizers.Add(swipeRightGesture);
                        radioButton.CheckedChanged += (sender, e) =>
                        {
                            if (e.Value)
                            {
                                RadioButtonCheckedChanged(sender, e);
                            }
                        };
                    }
                }

                //sprawdź czy radiobuttony zostały już kliknięte
                foreach (var grid in GridAnswer.Children)
                {
                    if (grid is MyRadioButton radioButton)
                    {
                        try
                        {
                            if (currentItem.RadioBtnSelected != null)
                            {
                                if (radioButton.Value.ToString() == currentItem.RadioBtnSelected)
                                {
                                    radioButton.IsChecked = true;
                                }
                                else
                                {
                                    radioButton.IsChecked = false;
                                }
                            }
                            else
                            {
                                radioButton.BorderColor = Color.FromHex("#2196F3");
                                radioButton.IsChecked = false;
                            }
                        }
                        catch (Exception ex) { Debug.WriteLine(ex); }
                    }
                }

                //sprawdzenie czy user udzielił odpowiedzi(dla natychmiastowego) lub czy kliknął w button Sprawdź 
                if (currentItem.Answer_checked == false)
                {
                    GridAnswer.IsEnabled = true;
                }
                else
                {
                    GridAnswer.IsEnabled = false;
                }

                // Scroll to center position in ContentView
                ItemsListView.ScrollTo(currentItem, position: ScrollToPosition.Center, animate: false);
            }
        }

        //function
        void OnSwiped(object sender, SwipedEventArgs e)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            int idexam = int.Parse(item.Id_Exam);
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    if (idexam < 40)
                    {
                        idexam += 1;
                        ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idexam - 1];
                    }
                    break;
                case SwipeDirection.Right:
                    if (idexam >1 )
                    {
                        idexam -= 1;
                        ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idexam - 1];
                    }
                    break;
            }
        }

        private void AnswerRight(string IdQuest)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest); //CurrentSelectionItemsListView

            if (OdpSwitch == true)
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).Answer_color = "niewiem";
            }
            else
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).Answer_color = "tak";

                if (ExpSwitch)
                {
                    if (item.Str_Explanation != "null")
                    {
                        LabelExplanation.Text = item.Str_Explanation;
                        FrameExplanation.IsVisible = true;
                        if (item.Exp_Picture != "null")
                        {
                            PictureExplanation.IsVisible = true;
                            PictureExplanation.Source = item.Exp_Picture;
                        }
                    }
                }
            }

            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is MyRadioButton radio)
                {
                    if (OdpSwitch == false)
                    {
                        if (radio.Value.ToString() == item.Answer_right.ToString())
                        {
                            radio.BorderColor = Color.Green;
                            break;
                        }
                    }
                    else
                    {
                        if (radio.Value.ToString() == item.Answer_user.ToString())
                        {
                            radio.BorderColor = Color.Orange;
                            break;
                        }
                    }
                    
                }
            }
        }

        private void AnswerWrong(string IdQuest)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest);

            if(OdpSwitch == true)
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).Answer_color = "niewiem";
            }
            else
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).Answer_color = "nie";
                
                if (item.Str_Explanation != "null")
                {
                    LabelExplanation.Text = item.Str_Explanation;
                    FrameExplanation.IsVisible = true;
                    if (item.Exp_Picture != "null")
                    {
                        PictureExplanation.IsVisible = true;
                        PictureExplanation.Source = item.Exp_Picture;
                    }
                }
            }
            
            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is MyRadioButton radio)
                {
                    if (OdpSwitch == false)
                    {
                        if (radio.Value.ToString() == item.Answer_user.ToString())
                        {
                            radio.BorderColor = Color.Red;
                        }
                        if (radio.Value.ToString() == item.Answer_right.ToString())
                        {
                            radio.BorderColor = Color.Green;
                        }
                    }
                    else
                    {
                        if (radio.Value.ToString() == item.Answer_user.ToString())
                        {
                            radio.BorderColor = Color.Orange;
                            break;
                        }
                    } 
                }
            }
        }

        private void GridDefaultRadioButtonBorderColor()
        {
            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is MyRadioButton radio)
                {
                    radio.BorderColor = Color.FromHex("#2196F3");
                }
            }
        }

        private void ItemsListViewChanged(object sender, SelectionChangedEventArgs e)
        {
            MySelectionChangedEventArgs = e;
            UpdateCurrentItemLabel();
            MySelectionChangedEventArgs = null;
        }

        private async void OnCountdownTick(object sender, int remainingSeconds)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Convert remaining seconds to hours, minutes, and seconds
                int hours = remainingSeconds / 3600;
                int minutes = remainingSeconds % 3600 / 60;
                int seconds = remainingSeconds % 60;

                CountdownLabel.Text = string.Format("{0:D2}:{1:D2}", minutes, seconds); //string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            });//string.Format("{0:D2}:{1:D2}", minutes, seconds)

            if (remainingSeconds == 0)
            {
                CountdownTimerProperty.Stop();
                koniecTestu = true;        
                CountdownLabel.Text = "00:00";
                await DisplayAlert("Informacja", "Koniec testu!!!", "OK");
                BtnSubmit_Clicked(null, null);
                koniecTestu = false;
            }
        }

        private async void BtnRefresh_Clicked(object sender, EventArgs e)
        {
            var refresh = await DisplayAlert("Uwaga!", "Czy chcesz wyczyścić wszystkie odpowiedzi i rozpocząć test od nowa?"
                , "Tak", "Nie");
            if (refresh == true)
                ResetExam("RefreshExam", true);
        }

        private async void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            bool refresh = false;
            if(koniecTestu == false)
            {
                refresh = await DisplayAlert("Uwaga!", "Czy chcesz zakończyć test?", "Tak", "Nie");
            }

            if (refresh == true || koniecTestu == true)
            {
                double goodAnswer=0;
                double wrongAnswer=0; 
                string examInfo;
                foreach (var currentItem in ViewModel.ItemsToViewModel)
                {
                    if(currentItem.Answer_right == currentItem.Answer_user)
                    {
                        goodAnswer++;
                    }
                    else
                    {
                        wrongAnswer++;
                    }
                }

                double result = Math.Round(goodAnswer / 40 * 100, 2);
                examInfo = $"Poprawne odpowiedzi: { goodAnswer}.\nBłędne odpowiedzi: {wrongAnswer}.\nTwój wynik: { result} %.\n";

                if (goodAnswer >= 26)
                {
                    examInfo += "Egzamin zaliczony.";
                }
                else
                {
                    examInfo += "Egzamin niezaliczony.";
                }
                await DisplayAlert("Wynik testu.", examInfo, "Ok");

                OdpSwitch = false;
                foreach (Item item in ViewModel.ItemsToViewModel)
                {
                    if (item.Answer_right == item.Answer_user)
                    {
                        item.Answer_color = "tak";
                    }
                    else
                    {
                        item.Answer_color = "nie";
                    }
                    item.Answer_checked = true;
                }
                //refresh view
                ChangeFrameBackgroundColor(true);
                // Set the first item as the selected item in the CollectionView
                ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
                CountdownTimerProperty.Stop();

                //if (refresh == true || koniecTestu == true)
                    await SaveToFileAsync();
            }
        }

        private async void BtnNewExam_Clicked(object sender, EventArgs e)
        {
            var refresh = await DisplayAlert("Uwaga!", "Czy chcesz wybrać nowy zestaw pytań?", 
                "Tak", "Nie");
            if (refresh == true)
            {
                ResetExam("NewExam", true);
            }
        }

        private void ResetExam(string examName, bool refresh)
        {
            OdpSwitch = (bool)Application.Current.Properties["OdpSwitch"];
            if (refresh)
            {
                if(examName == "RefreshExam")
                {
                    foreach (Item item in ViewModel.ItemsToViewModel)
                    {
                        item.Answer_user = null;
                        item.RadioBtnSelected = null;
                        item.Answer_clicked = false;
                        item.Answer_color = null;
                        item.IsSelected = false; //dodałem tą opcję 14.11 o 00:37
                        item.Answer_checked = false;
                    }
                }
                else
                {
                    //BindingContext = ViewModel = null;
                    BindingContext = ViewModel = new ExamViewModel("exam", string.Empty);
                    OnAppearing();
                }

                countClickedAnswer = 0; CountQuestions.Text = "0/40";
                CountdownTimerProperty.Stop();
                CountdownTimerProperty = new CountdownTimer(CountDownTime); // Set the countdown time in seconds
                CountdownTimerProperty.CountdownTick += OnCountdownTick;

                CountdownTimerProperty.Start();
            }
            // Set the first item as the selected item in the CollectionView
            ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
            ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
            ChangeFrameBackgroundColor(true);
        }

        private void ChangeFrameBackgroundColor(bool reset)
        {
            // Zmiana kolotu tła dla udzielonej odpowiedzi - kliknięcie radiobutton
            IList<Item> items = ViewModel.ItemsToViewModel;
            // Ręczne odświeżenie widoku
            ItemsListView.ItemsSource = null;
            ItemsListView.ItemsSource = items;
            if (reset)
            {
                ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
            }
            // Scroll to center position in ContentView
            ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView)
                , position: ScrollToPosition.Center, animate: false);
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            int idexam = int.Parse(item.Id_Exam);

            ImageButton imageButton = (ImageButton)sender;
            if(imageButton.TabIndex == 1)
            {
                if (idexam > 1)
                {
                    idexam -= 1;
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idexam-1];
                }
            }
            else
            {
                if (idexam < 40)
                {
                    idexam += 1;
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idexam-1];
                }
            }
        }

        private async Task SaveToFileAsync()
        {
            await Task.Run(() => // Uruchamiamy zadanie w innym wątku
            {
                //pobranie i zapisanie numeru egzaminu 
                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                    LocalApplicationData), ExamFileNumber);
                //File.Delete(FilePath);
                // create file if not exist
                if (!File.Exists(FilePath))
                {
                    ExamNumber = 1;
                    File.WriteAllText(FilePath, $"{ExamNumber}", System.Text.Encoding.Default);
                }
                else
                {
                    string readContent = File.ReadAllText(FilePath);

                    if (readContent != "")
                    {
                        // Znalezienie największej wartości
                        ExamNumber = int.Parse(readContent) + 1;
                    }
                    else
                    {
                        ExamNumber = 1;

                    }
                    File.WriteAllText(FilePath, $"{ExamNumber}", System.Text.Encoding.Default);
                }
                // koniec działań na pliku ExamFileNumber

                //zapis danych egzaminu, 40 wierszy, pierwsza wartość to nr egzaminu, potem wartości pól z Item
                //ExamNumber:item.Id_Exam:item.Id:item.Answer_user:item.RadioBtnSelected:item.Answer_clicked:item.IsSelected:item.Answer_checked:color(tak/nie)
                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                    LocalApplicationData), ExamFileName);
                //File.Delete(FilePath);
                string ExamItemString = string.Empty;

                foreach(var item in ViewModel.ItemsToViewModel)
                {
                    ExamItemString = string.Empty;
                    ExamItemString = $"{ExamNumber}:{item.Id_Exam}:{item.Id}:{item.Answer_user}:{item.RadioBtnSelected}:{item.Answer_clicked}:{item.IsSelected}:{item.Answer_checked}:{item.Answer_color}";

                    // Tu wykonujemy operację zapisu do pliku
                    if (!File.Exists(FilePath))
                    {
                        File.WriteAllText(FilePath, $"{ExamItemString}\n\r", System.Text.Encoding.Default);

                    }
                    else
                    {
                        File.AppendAllText(FilePath, $"{ExamItemString}\n\r", System.Text.Encoding.Default);
                    }
                }
            });
        }

        [Obsolete]
        private async void OnImageTappedQuestions(object sender, EventArgs e)
        {
            // Tutaj możesz otworzyć nową stronę z powiększonym obrazem
            //await Navigation.PushAsync(new ZoomPanImageView($"{zoomedImageQuestions}"));
            await PopupNavigation.Instance.PushAsync(new PhotoPopupPage(zoomedImageQuestions));
        }

        [Obsolete]
        private async void OnImageTappedExplanation(object sender, EventArgs e)
        {
            // Tutaj możesz otworzyć nową stronę z powiększonym obrazem
            //await Navigation.PushAsync(new ZoomPanImageView($"{zoomedImageExpanation}"));
            await PopupNavigation.Instance.PushAsync(new PhotoPopupPage(zoomedImageExplanation));
        }
    }

    public class CountdownTimer
    {
        private int countdownSeconds;
        private readonly Timer timer;
        private bool isPaused;

        public event EventHandler<int> CountdownTick;
        public event EventHandler CountdownFinished;

        public CountdownTimer(int seconds)
        {
            countdownSeconds = seconds;
            timer = new Timer(1000); // Timer ticks every 1 second (1000 milliseconds)
            timer.Elapsed += TimerElapsed;
        }

        public void Start()
        {
            if (!isPaused)
            {
                timer.Start();
            }
            else
            {
                Resume();
            }
        }

        public void Pause()
        {
            isPaused = true;
            timer.Stop();
        }

        public void Resume()
        {
            isPaused = false;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            isPaused = false;
            countdownSeconds = 0;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //countdownSeconds--;

            //CountdownTick?.Invoke(this, countdownSeconds);

            //if (countdownSeconds <= 0)
            //{
            //    Stop();
            //    CountdownFinished?.Invoke(this, EventArgs.Empty);
            //}
            Device.BeginInvokeOnMainThread(() =>
            {
                if (countdownSeconds > 0)
                {
                    countdownSeconds--;

                    CountdownTick?.Invoke(this, countdownSeconds);
                }
                else
                {
                    Stop();
                    CountdownFinished?.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }

}