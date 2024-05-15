using ISTQB_PL.Models;
using ISTQB_PL.ViewModels;
using System.Diagnostics;
using System;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using Color = Xamarin.Forms.Color;
using System.IO;
using ISTQB_PL.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;

namespace ISTQB_PL.Views
{
    public partial class ItemsPageWrongAnswer : ContentPage
    {
        private ExamViewModel ViewModel { get; set; }
        private SelectionChangedEventArgs MySelectionChangedEventArgs { get; set; }

        private int MyFontSize { get; set; }
        private bool OdpSwitch { get; set; }
        private bool ExpSwitch { get; set; }

        private string zoomedImageQuestions;
        private string zoomedImageExplanation;

        MyRadioButton NewRadioA { get; set; }
        MyRadioButton NewRadioB { get; set; }
        MyRadioButton NewRadioC { get; set; }
        MyRadioButton NewRadioD { get; set; }
        Button BtnAnswer { get; set; }

        private string CurrentSelectionItemsListView = null;

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public ItemsPageWrongAnswer()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                BindingContext = ViewModel = new ExamViewModel("itemswrong", string.Empty);

                this.Appearing += OnPageAppearing;
            }
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        if (ViewModel.ItemsToViewModel.Count > 0)
                        {
                            ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.LastOrDefault());
                            ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.FirstOrDefault());
                        }
                    }
                });
                //return false aby zatrzymać timer
                return false;
            });
        }

        protected override void OnAppearing()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    base.OnAppearing();

                    MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
                    MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
                    foreach (var item in ViewModel.ItemsToViewModel)
                    {
                        item.ItemMainTextColor = MainTextColor;
                    }
                    ViewModel.OnAppearing();

                    MyFontSize = Application.Current.Properties.ContainsKey("FontSize") ?
                        int.Parse(Application.Current.Properties["FontSize"].ToString()) : 18;

                    OdpSwitch = Application.Current.Properties.ContainsKey("OdpSwitch")
                        && (bool)Application.Current.Properties["OdpSwitch"];

                    ExpSwitch = Application.Current.Properties.ContainsKey("ExpSwitch")
                        && (bool)Application.Current.Properties["ExpSwitch"];

                    MyLabel.FontSize = MyFontSize;
                    MyContentLabel.FontSize = MyFontSize;
                    LabelExplanation.FontSize = MyFontSize;
                    // Refresh the selected item in the CollectionView
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
                }
                //return false aby zatrzymać timer
                return false;
            });
        }

        private void RadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value != false)
            {
                GridDefaultRadioButtonBorderColor(); //defaultowe ustawienia radiobuttonów

                MyRadioButton radioButton = sender as MyRadioButton;

                Item currentItem = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView);
                //ViewModel.Items.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).IsSelected = true;
                if (CurrentSelectionItemsListView == currentItem.Id)
                {
                    currentItem.Answer_user = radioButton.Value.ToString(); //added user_answer to binding
                    currentItem.RadioBtnSelected = radioButton.Value.ToString();  // which radiobutton was clicked added to binding
                    currentItem.Answer_clicked = true;
                    currentItem.IsSelected = true;

                    if (!OdpSwitch)
                    { //dodanie info o tym, że odpowiedź już była udzielona, żeby zablokować AnswerGrid
                        currentItem.Answer_checked = true;
                        if (BtnAnswer != null)
                        {
                            BtnAnswer.IsEnabled = false;
                        }
                        GridAnswer.IsEnabled = false;
                    }
                    //sprawdzenie odpowiedzi, na zielone jeżeli prawdidłowa, błędna odpowiedź - zaznaczenie dwóch RB, prawidłowa odpowiedź na zielono, błędna na czerwono 
                    if (currentItem.Answer_user.ToLower() == currentItem.Answer_right.ToLower())
                    {
                        AnswerRight(currentItem.Id, OdpSwitch);
                    }
                    else
                    {
                        AnswerWrong(currentItem.Id, OdpSwitch);
                    }
                }
                ChangeFrameBackgroundColor(false);
                if (BtnAnswer != null)
                {
                    BtnAnswer.IsEnabled = true;
                }
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
                if (currentItem.Str_Picture != "null") //sprawdź czy należy wyświetlić zdjęcie
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
                MyLabel.Text = $"Pytanie: {currentItem.Id_Sorted}, (Sylabus: {currentItem.Wersja_Sylabus[..3]}, Rozdział: {currentItem.Rozdzial})"; //, Nr pytania: {currentItem.Id}
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
                    FontSize = MyFontSize
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
                ItemsListView.ScrollTo(currentItem, position: ScrollToPosition.Center, animate: false);
            }
        }

        //function
        void OnSwiped(object sender, SwipedEventArgs e)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            int idquestions = int.Parse(item.Id_Wrong);
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    if (idquestions < ViewModel.ItemsToViewModel.Count)
                    {
                        idquestions += 1;
                        ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idquestions - 1];
                    }
                    break;
                case SwipeDirection.Right:
                    if (idquestions > 1)
                    {
                        idquestions -= 1;
                        ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idquestions - 1];
                    }
                    break;
            }
        }

        private void AnswerRight(string IdQuest, bool odpSwitch)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest); //CurrentSelectionItemsListView

            if (odpSwitch == true && (item.Answer_color == "null" || item.Answer_color == "niewiem"))
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

            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - czerwony/zielony/pomarańczowy
            {
                if (grid is MyRadioButton radio)
                {
                    if (odpSwitch == false && item.Answer_color == "tak")
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
                            if (item.Answer_color == "tak")
                            {
                                radio.BorderColor = Color.Green;
                            }
                            else
                            {
                                radio.BorderColor = Color.Orange;
                            }
                            break;
                        }
                    }

                }
            }
        }

        private void AnswerWrong(string IdQuest, bool odpSwitch)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest);

            if (odpSwitch == true && (item.Answer_color == "null" || item.Answer_color == "niewiem"))
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

            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - czerwony/zielony/pomarańczowy
            {
                if (grid is MyRadioButton radio)
                {
                    if (odpSwitch == false && item.Answer_color == "nie")
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
                        if (odpSwitch == true && item.Answer_color == "nie")
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
            int idquestions = int.Parse(item.Id_Wrong);

            ImageButton imageButton = (ImageButton)sender;
            if (imageButton.TabIndex == 1)
            {
                if (idquestions > 1)
                {
                    idquestions -= 1;
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idquestions - 1];
                }
            }
            else
            {
                if (idquestions < ViewModel.ItemsToViewModel.Count)
                {
                    idquestions += 1;
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[idquestions - 1];
                }
            }
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
}

