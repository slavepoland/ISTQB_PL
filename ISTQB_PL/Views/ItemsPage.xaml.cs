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
    public partial class ItemsPage : ContentPage
    {
        private readonly string ItemsFileNumber = "itemsnumber.txt";
        private readonly string ItemsFileName = "answers.txt";
        private string FilePath;

        private string zoomedImageQuestions;
        private string zoomedImageExplanation;

        private ExamViewModel ViewModel { get; set; }
        private SelectionChangedEventArgs MySelectionChangedEventArgs { get; set; }

        private int MyFontSize { get; set; }
        private bool OdpSwitch { get; set; }
        private bool ExpSwitch { get; set; }

        MyRadioButton NewRadioA { get; set; }
        MyRadioButton NewRadioB { get; set; }
        MyRadioButton NewRadioC { get; set; }
        MyRadioButton NewRadioD { get; set; }
        Button BtnAnswer { get; set; }

        private string CurrentSelectionItemsListView = null;

        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public ItemsPage()
        {
            InitializeComponent();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            else
            {
                BindingContext = ViewModel = new ExamViewModel("items", string.Empty);
                this.Appearing += OnPageAppearing;

                FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                    LocalApplicationData), ItemsFileNumber);
                try
                {
                    File.WriteAllText(FilePath, ViewModel.Items.Count().ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        private async void MyDisplayAlert(string content, string message)
        {
            await DisplayAlert("Błąd", $"{content} Błąd: {message}", "OK");
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

                            var selectedViewModelItem = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView);
                            if (selectedViewModelItem != null)
                            {
                                ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.LastOrDefault(), position: ScrollToPosition.Center, animate: false);
                                ItemsListView.ScrollTo(selectedViewModelItem, position: ScrollToPosition.Center, animate: false);
                            }
                            break;
                        }
                    }
                }
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

                    //sprawdzenie, na które pytania user już odpowiedział
                    //Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected 
                    FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                        LocalApplicationData), ItemsFileName);
                    //File.Delete(filePath);
                    if (File.Exists(FilePath))
                    {
                        string[] readContent = ReadAndSplitFile(FilePath);
                        if (readContent.Count() > 0) //&& readContent[0] != ""
                        {
                            foreach (string line in readContent)
                            {//Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected:Answer_Checked
                                try
                                {
                                    string[] answer = line.Split(':');
                                    ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).Answer_user = answer[1];
                                    ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).Answer_right = answer[2];
                                    try
                                    {
                                        bool answer_clicked = bool.TryParse(answer[3].ToLower(), out answer_clicked);
                                        ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).Answer_clicked =
                                            answer_clicked;
                                    }
                                    catch (Exception ex)
                                    {
                                        MyDisplayAlert("Answer_clicked.(Konstruktor ItemsPage).", ex.Message);
                                    }

                                    ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).Answer_color = answer[4];
                                    ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).RadioBtnSelected = answer[5];
                                    try
                                    {
                                        bool isselected = bool.TryParse(answer[6].ToLower(), out isselected);
                                        ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).IsSelected = isselected;
                                    }
                                    catch (Exception ex)
                                    {
                                        MyDisplayAlert("IsSelected.(Konstruktor ItemsPage).", ex.Message);
                                    }
                                    try
                                    {
                                        bool answer_checked = bool.TryParse(answer[7].ToLower(), out answer_checked);
                                        ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == answer[0].ToString()).Answer_checked = answer_checked;
                                    }
                                    catch (Exception ex)
                                    {
                                        MyDisplayAlert("answer_checked.(Konstruktor ItemsPage).", ex.Message);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MyDisplayAlert("Wczytanie poprzednich odpowiedzi(Konstruktor ItemsPage).", ex.Message);
                                }
                            }
                        }
                    }

                    MyLabel.FontSize = MyFontSize;
                    MyContentLabel.FontSize = MyFontSize;
                    LabelExplanation.FontSize = MyFontSize;

                    // Refresh the selected item in the CollectionView
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.FirstOrDefault();
                    ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.LastOrDefault(), position: ScrollToPosition.Center, animate: false);
                    ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.FirstOrDefault(), position: ScrollToPosition.Center, animate: false);
                    //return false aby zatrzymać timer
                }
                return false;
            });
        }

        private void RadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value != false)
            {
                GridDefaultRadioButtonBorderColor(); //defaultowe ustawienia radiobuttonów

                MyRadioButton radioButton = sender as MyRadioButton;

                Item currentItem = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView);
                //ViewModel.Items.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView).IsSelected = true;
                if (CurrentSelectionItemsListView == currentItem.Id_Sorted)
                {
                    currentItem.Answer_user = radioButton.Value.ToString(); //added user_answer to binding
                    currentItem.RadioBtnSelected = radioButton.Value.ToString();  // which radiobutton was clicked added to binding
                    currentItem.Answer_clicked = true;
                    currentItem.Answer_checked = true;
                    currentItem.IsSelected = true;

                    if(!OdpSwitch)
                    { //dodanie info o tym, że odpowiedź już była udzielona, żeby zablokować AnswerGrid
                        currentItem.Answer_checked = true;
                        if (BtnAnswer != null)
                        {
                            BtnAnswer.IsEnabled = false;
                        }
                        GridAnswer.IsEnabled = false;
                    }
                    //sprawdzenie odpowiedzi, na zielono jeżeli prawdidłowa, błędna odpowiedź - zaznaczenie dwóch RB, prawidłowa odpowiedź na zielone, błędna na czerwono 
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
                if(BtnAnswer != null)
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
                CurrentSelectionItemsListView = currentItem.Id_Sorted;
            }
            else
            {
                currentItem = ViewModel.ItemsToViewModel[0];
                CurrentSelectionItemsListView = ViewModel.ItemsToViewModel[0].Id_Sorted;
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
                    TextColor =MainTextColor,
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

                if (OdpSwitch) //nie udzielaj odpowiedzi natychmiast, we wszystkich pytaniach po kliknieciu Sprawdz, w egzaminie po zakończeniu testu
                {
                    BtnAnswer = new Button
                    {
                        Text = "Sprawdź",
                        BackgroundColor = Color.FromHex("#2196F3"),
                        TextColor = Color.White,
                        FontSize = MyFontSize,
                        IsVisible = false,
                        Command = new Command(BtnAnswer_Clicked),
                        IsEnabled = false,
                    };

                    var swipeLeftGestureBtn = new SwipeGestureRecognizer
                    {
                        Direction = SwipeDirection.Left
                    };

                    var swipeRightGestureBtn = new SwipeGestureRecognizer
                    {
                        Direction = SwipeDirection.Right
                    };
                    swipeLeftGestureBtn.Swiped += OnSwiped;
                    swipeRightGestureBtn.Swiped += OnSwiped;
                    BtnAnswer.GestureRecognizers.Add(swipeLeftGestureBtn);
                    BtnAnswer.GestureRecognizers.Add(swipeRightGestureBtn);

                    BtnAnswer.IsVisible = true;
                    GridAnswer.Children.Add(BtnAnswer, 0, 4);
                }  //if OdpSwitch is true, twórz BtnAnswer          


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

                //sprawdzenie czy user udzielił odpowiedzi(dla natychmiastowego) lub czy kliknął w button "Sprawdź" 
                switch (currentItem.Answer_checked)
                {
                    case true:
                        if (BtnAnswer != null)
                        { 
                            switch (currentItem.Answer_color)
                            {
                                case "niewiem":
                                    BtnAnswer.IsEnabled = true;
                                    GridAnswer.IsEnabled = true;
                                    break;
                                case "null":
                                    BtnAnswer.IsEnabled = true;
                                    GridAnswer.IsEnabled = true;
                                    break;
                                case "tak":
                                    BtnAnswer.IsEnabled = false;
                                    GridAnswer.IsEnabled = false;
                                    break;
                                case "nie":
                                    BtnAnswer.IsEnabled = false;
                                    GridAnswer.IsEnabled = false;
                                    break;
                            }
                        }
                        break;
                    case false:
                        if (BtnAnswer != null)
                        {
                            BtnAnswer.IsEnabled = false;
                        }
                        GridAnswer.IsEnabled = true;
                        break;
                }
                // Scroll to center position in ContentView
                ItemsListView.ScrollTo(currentItem, position: ScrollToPosition.Center, animate: false);
            }
        }

        //function
        void OnSwiped(object sender, SwipedEventArgs e)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            int idquestions = int.Parse(item.Id_Sorted);
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

        private string[] ReadAndSplitFile(string filePath)
        {
            try
            {
                // Odczyt z pliku
                string fileContent = File.ReadAllText(filePath);

                // Podział na string[] na podstawie znaków nowej linii
                string[] lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                return lines;
            }
            catch (Exception ex)
            {
                MyDisplayAlert("Błąd podczas odczytu pliku(ReadAndSplitFile):", ex.Message);
                return new string[0]; // Zwrócenie pustej tablicy w przypadku błędu
            }
        }

        static string[] CreateNewArray(string[] array, int numberToFind, Item itemViewModel)
        {
                // Aktualizacja istniejącego wiersza
                return array.Select(item =>
                {
                    string[] parts = item.Split(':');
                    if (parts.Length >= 1 && int.TryParse(parts[0], out int number) && number == numberToFind)
                    {
                        // Aktualizuj tylko wiersz z daną liczbą
                        string contentToAdd = $"{itemViewModel.Id}:{itemViewModel.Answer_user}:{itemViewModel.Answer_right}:" +
                                              $"{itemViewModel.Answer_clicked}:{itemViewModel.Answer_color}:{itemViewModel.RadioBtnSelected}:" +
                                              $"{itemViewModel.IsSelected}:{itemViewModel.Answer_checked}";
                        return contentToAdd;
                    }
                    else
                    {
                        return item;
                    }
                }).ToArray(); 
        }

        private void SaveUserAnswerInFile(Item itemViewModel)
        {
            if (File.Exists(FilePath))
            {
                string contentToAdd;
                int numberToFind = int.Parse(itemViewModel.Id);

                string[] readContent = ReadAndSplitFile(FilePath);

                // Sprawdzenie, czy liczba(czyli numer pytania) istnieje w tablicy
                bool numberExists = readContent.Any(item =>
                {
                    string[] parts = item.Split(':');
                    return parts.Length >= 1 && int.TryParse(parts[0], out int number) 
                    && number == numberToFind;
                });
                if (numberExists)
                {
                    // Jeśli liczba istnieje(czyli user już odpowiadał na pytanie), zwróć tablicę bez zmian, ale zaaktualizuj odpowiedź usera, bo mógł zmienić
                    string[] newReadContent = CreateNewArray(readContent, numberToFind, itemViewModel);
                    try
                    {
                        File.WriteAllLines(FilePath, newReadContent, System.Text.Encoding.Default);
                    }
                    catch (Exception ex) { Debug.WriteLine(ex); }
                }
                else
                {//Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected:Answer_Checked
                    // Dodaj nowe dane na końcu pliku, czyli na dane pytanie user jeszcze nie odpowiadał
                    contentToAdd = $"{itemViewModel.Id}:{itemViewModel.Answer_user}:{itemViewModel.Answer_right}:" +
                                   $"{itemViewModel.Answer_clicked}:{itemViewModel.Answer_color}:{itemViewModel.RadioBtnSelected}:" +
                                   $"{itemViewModel.IsSelected}:{itemViewModel.Answer_checked}";
                    try
                    {
                        File.AppendAllText(FilePath, $"{contentToAdd}\n\r", System.Text.Encoding.Default);
                    }
                    catch(Exception ex) { Debug.WriteLine(ex); }
                }
            }
            else
            {
                //żaden plik czyli na żadne pytanie nie było odpowiedzi)albo nowa instalka, albo wyczyszczenie przez usera odpowiedzi
                string contentToAdd;
                //Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected:Answer_Checked
                contentToAdd = $"{itemViewModel.Id}:{itemViewModel.Answer_user}:{itemViewModel.Answer_right}:" +
                               $"{itemViewModel.Answer_clicked}:{itemViewModel.Answer_color}:{itemViewModel.RadioBtnSelected}:" +
                               $"{itemViewModel.IsSelected}:{itemViewModel.Answer_checked}";
                try
                {
                    if (contentToAdd != "")
                    {
                        try
                        {
                            File.WriteAllText(FilePath, $"{contentToAdd}\n\r", System.Text.Encoding.Default);
                        }
                        catch(Exception ex) { Debug.WriteLine(ex); }
                    }
                }
                catch (Exception ex)
                {
                    MyDisplayAlert("Nie udało stworzyć pliku.", ex.Message);
                }
            }
        }

        private void AnswerRight(string IdQuest, bool odpSwitch)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest); //CurrentSelectionItemsListView

            if (odpSwitch == true && (item.Answer_color == "null" || item.Answer_color == "niewiem"))
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView).Answer_color = "niewiem";
            }
            else
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView).Answer_color = "tak";

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
                            if(item.Answer_color == "tak")
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
            SaveUserAnswerInFile(item);
        }

        private void AnswerWrong(string IdQuest, bool odpSwitch)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id == IdQuest);

            if (odpSwitch == true && (item.Answer_color == "null" || item.Answer_color == "niewiem"))
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView).Answer_color = "niewiem";
            }
            else
            {
                ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView).Answer_color = "nie";

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
            SaveUserAnswerInFile(item);
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
            ItemsListView.ScrollTo(ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView)
                , position: ScrollToPosition.Center, animate: false);
        } 

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            int idquestions = int.Parse(item.Id_Sorted);

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

        private void BtnAnswer_Clicked()
        {
            if(BtnAnswer != null)
            {
                var item = ViewModel.ItemsToViewModel.FirstOrDefault(x => x.Id_Sorted == CurrentSelectionItemsListView);
                item.Answer_checked = true;
                if (item.Answer_user.ToLower() == item.Answer_right.ToLower())
                {

                    AnswerRight(item.Id, false);
                }
                else
                {
                    AnswerWrong(item.Id, false);
                }
                ChangeFrameBackgroundColor(false);
                GridAnswer.IsEnabled = false;
                BtnAnswer.IsEnabled = false;
            }    
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Uwaga!", "Czy na pewno chcesz usunąć odpowiedzi?", "Tak", "Nie");
            if(result)
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch(Exception ex)
                {
                    MyDisplayAlert("Nie udało się usunąć pliku z odpowiedziami.", ex.Message);
                }
                ViewModel = new ExamViewModel("items", string.Empty);
                ChangeFrameBackgroundColor(true);
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

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Pobierz wprowadzony tekst z SearchBar
            string searchText = e.NewTextValue;

            // Sprawdź, czy każdy znak to cyfra
            if (!string.IsNullOrEmpty(searchText))
            {
                foreach (char c in searchText)
                {
                    if (!char.IsDigit(c))
                    {
                        // Jeśli znaleziono znak, który nie jest cyfrą, usuń ten znak z tekstu
                        searchText = searchText.Replace(c.ToString(), string.Empty);
                    }
                }
            }
            // Ogranicz tekst do maksymalnie 5 cyfr
            if (searchText.Length > 5)
            {
                searchText = searchText[..5];
            }
            // Zapewnij, że pierwsza cyfra nie jest zerem
            if (searchText.Length > 0 && searchText[0] == '0')
            {
                searchText = searchText.Substring(1);
            }
            // Ustaw oczyszczony tekst z powrotem do SearchBar
            mySearchBar.Text = searchText;

            if (searchText.Length > 0)
            {
                int QuestionNumber = int.Parse(searchText);

                if (QuestionNumber >= 1 && QuestionNumber <= ViewModel.Items.Count())
                {
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel[QuestionNumber - 1];
                    ItemsListView.ScrollTo(ViewModel.ItemsToViewModel[QuestionNumber - 1], position: ScrollToPosition.Center, animate: false);
                }
                else if (QuestionNumber > ViewModel.Items.Count())
                {
                    ItemsListView.SelectedItem = ViewModel.ItemsToViewModel.LastOrDefault();
                }
            }
            else
            {
                OnPageAppearing(null, null);
            }
        }
    }
}

