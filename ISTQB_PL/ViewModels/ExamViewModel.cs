using Xamarin.Forms;
using System.Diagnostics;
using System;
using ISTQB_PL.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ISTQB_PL.Services;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ISTQB_PL.ViewModels
{
    //[QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ExamViewModel : BaseViewModel
    {
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Item> GroupedRows { get; set; }
        public ObservableCollection<Item> ItemsToViewModel { get; set; }
        public ObservableCollection<Item> ItemsWrong { get; set; }
        public ObservableCollection<Item> ItemsExam { get; set; }
        public Dictionary<int, int> KeyValuesRozdzial { get; set; } //8 + 5 + 5 + 11 + 9 + 2 = 40

        private readonly INavigationService _navigationService;
        public Command LoadItemsCommand { get; }
        public Command ExamGoBackCommand { get; }
        public Command GoBackCommand { get; }

        private readonly string WrongAnswerFileName = "answers.txt";
        private readonly string ExamFileName = "exam.txt";
        private int ExamNumber { get; set; }

        private string FilePath;

        public ExamViewModel(string itemsorexam, string examnumber)
        {
            if(examnumber != string.Empty)
                ExamNumber = int.Parse(examnumber);

            _navigationService = DependencyService.Get<INavigationService>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand("exam"));

            ExamGoBackCommand = new Command(async  () => await MainExamGoBackCommand());

            GoBackCommand = new Command(async () => await MainGoBackCommand());

            Items = new ObservableCollection<Item>();
            ItemsToViewModel = new ObservableCollection<Item>();

            //load data to CollectionView.ItemSource
            _ = ExecuteLoadItemsCommand(itemsorexam);
            //mainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            switch (itemsorexam)
            {
                case "items": Title = $"Manual tester-pytania({ItemsToViewModel.Count()})"; break;
                case "exam": Title = "Manual tester-egzamin"; break;
                case "itemswrong": Title = $"Błędne odpowiedzi({ItemsToViewModel.Count()})"; break;
                case "examsaved": Title = "Zapisane egzaminy"; break;
                case "examsaveddetail": Title = $"Egzamin nr {ExamNumber}"; break;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        public async Task MainExamGoBackCommand()
        {
            IsBusy = true;
            try
            {
                var alertService = DependencyService.Get<IAlertService>();
                bool result = await alertService.DisplayAlertCommit("Uwaga!", "Czy chcesz zamknąć test? Dane nie zostaną zapamiętane.", "Tak", "Nie");

                if (result == true)
                {
                    _navigationService.GoBackToAboutPage();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task MainGoBackCommand()
        {
            IsBusy = true;
            try
            {
                _navigationService.GoBackToAboutPage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteLoadItemsCommand(string itemsorexam)
        {
            IsBusy = true;
            try
            {
                ItemsToViewModel.Clear();
                Items = await DataStore.GoogleSheetsExam(true);
                Items = Items.OrderBy(item => item.Wersja_Sylabus)
                                                   .ThenBy(item => item.Rozdzial)
                                                   .ToList();
                int j = 1;
                foreach (var item in Items)
                {
                    item.Id_Sorted = j.ToString();
                    j++;
                }
                switch (itemsorexam)
                {
                    case "exam": //8 + 5 + 5 + 11 + 9 + 2 = 40
                        KeyValuesRozdzial = new Dictionary<int, int>
                        {
                            {1, 8},
                            {2, 5},
                            {3, 5},
                            {4, 11},
                            {5, 9},
                            {6, 2}
                        };
                        //var numbers = RandomNumberGenerator.GenerateUniqueRandomNumbers
                        //(1, Items.Count(), 40);
                        ////string[] numbers = {"128", "129", "58", "80", "75", "3", "20" }; //for testing
                        //foreach (var number in numbers)
                        //{
                        //    ItemsToViewModel.Add(Items.FirstOrDefault(s => s.Id == number));
                        //}
                        foreach (var item in KeyValuesRozdzial)
                        {
                            if (Application.Current.Properties["Wersja3"].ToString() == "tak" 
                                && Application.Current.Properties["Wersja4"].ToString() == "nie")
                            {
                                GroupedRows = Items.Where(x => x.Wersja_Sylabus == "3.1.1.8" && x.Rozdzial == item.Key.ToString());
                                //GroupedRows = Items.Where(x => x.Rozdzial == item.Key.ToString());
                            }
                            else if(Application.Current.Properties["Wersja3"].ToString() == "nie" 
                                && Application.Current.Properties["Wersja4"].ToString() == "tak")
                            {
                                GroupedRows = Items.Where(x => x.Wersja_Sylabus == "4.0.0.1" && x.Rozdzial == item.Key.ToString());
                                //GroupedRows = Items.Where(x => x.Rozdzial == item.Key.ToString());
                            }
                            else
                            {
                                GroupedRows = Items.Where(x => x.Rozdzial == item.Key.ToString());
                            }
                            
                            //var numbers = RandomNumberGenerator.GenerateUniqueRandomNumbers
                            //(1, groupedRows.Count() + 1, item.Value);

                            Random random = new Random();
                            // Sortowanie z dwoma poziomami losowości
                            GroupedRows = GroupedRows.OrderBy(x => random.Next()).ThenBy(x => random.Next());

                            // Wylosuj 8 wierszy dla każdego rozdziału
                            var selectedRows = GroupedRows.Take(item.Value).ToList();
                            foreach(var item1 in selectedRows)
                            {
                                ItemsToViewModel.Add(item1);
                            }
                        }

                        int i = 1;
                        foreach (var item in ItemsToViewModel)
                        {
                            item.Id_Exam = i.ToString();
                            i++;
                        }
                        break;
                    case "items":
                        foreach(var item in Items)
                        {
                            ItemsToViewModel.Add(item);
                        }
                        break;
                    case "itemswrong":
                        ItemsWrong = new ObservableCollection<Item>();
                        UserWrongAnswersAddToViewModel();
                        ItemsToViewModel = ItemsWrong;
                        break;
                    case "examsaved":
                        foreach (var item in Items)
                        {
                            ItemsToViewModel.Add(item);
                        }
                        break;
                    case "examsaveddetail":
                        ItemsExam = new ObservableCollection<Item>();
                        UserAnswerExamAddToViewModel();
                        ItemsToViewModel = ItemsExam;
                        break;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
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
                Debug.WriteLine("Błąd podczas odczytu pliku(ReadAndSplitFile):", ex.Message);
                return new string[0]; // Zwrócenie pustej tablicy w przypadku błędu
            }
        }

        private void UserWrongAnswersAddToViewModel()
        {
            //sprawdzenie, na które pytania user już odpowiedział
            //Id:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected:Answer_Checked
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), WrongAnswerFileName);
            if (File.Exists(FilePath))
            {
                //string[] readContent = ReadAndSplitFile(FilePath);
                List<string> lines = File.ReadAllLines(FilePath).ToList();

                // Przetwórz dane i utwórz listę obiektów AnswerData
                List<AnswerData> answersList = new List<AnswerData>();
                foreach (var line in lines)
                {
                    if(line != "")
                    {
                        string[] parts = line.Split(':');
                        var answer = new AnswerData
                        {
                            Id = int.Parse(parts[0]),
                            AnswerUser = parts[1],
                            AnswerRight = parts[2],
                            AnswerClicked = parts[3],
                            Color = parts[4],
                            RadioBtnSelected = parts[5],
                            IsSelected = parts[6],
                            Answer_checked = parts[7]
                        };
                        answer.Id_Sorted = int.Parse(Items.FirstOrDefault(x => x.Id == parts[0].ToString()).Id_Sorted);
                        answersList.Add(answer);
                    } 
                }
                // Posortuj listę rosnąco według Id
                answersList = answersList.OrderBy(a => a.Id_Sorted).ToList();

                if (answersList.Count() > 0)
                {
                    int id_wrong = 1;
                    foreach (var line in answersList)
                    {
                        try
                        {
                            //string[] answer = line.Split(':');
                            if (line.AnswerRight != line.AnswerUser) //add to ItemsWrong,only wrong answers 
                            {//Id:Id_Sorted:Answer_user:Answer_right:Answer_clicked:color(tak/nie):RadioBtnSelected:IsSelected:Answer_Checked
                                ItemsWrong.Add(Items.FirstOrDefault(x => x.Id == line.Id.ToString()));

                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Id_Sorted = line.Id_Sorted.ToString();
                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Answer_user = line.AnswerUser;
                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Answer_right = line.AnswerRight;
                                try
                                {
                                    bool answer_clicked = bool.TryParse(line.AnswerClicked.ToLower(), out answer_clicked);
                                    ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Answer_clicked =
                                        answer_clicked;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Answer_clicked.(ExamViewModel WrongAnswers).", ex.Message);
                                }

                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Answer_color = line.Color;
                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).RadioBtnSelected = line.RadioBtnSelected;
                                try
                                {
                                    bool isselected = bool.TryParse(line.IsSelected.ToLower(), out isselected);
                                    ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).IsSelected = isselected;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("IsSelected.(ExamViewModel WrongAnswers).", ex.Message);
                                }
                                try
                                {
                                    bool answer_checked = bool.TryParse(line.Answer_checked.ToLower(), out answer_checked);
                                    ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Answer_checked = answer_checked;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("answer_checked.(ExamViewModel WrongAnswers).", ex.Message);
                                }
                                ItemsWrong.FirstOrDefault(x => x.Id == line.Id.ToString()).Id_Wrong = id_wrong.ToString();
                                id_wrong++;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Wczytanie poprzednich odpowiedzi(ExamViewModel WrongAnswers).", ex.Message);
                        }
                    }
                }
            }
        }

        private void UserAnswerExamAddToViewModel()
        {
            FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.
                LocalApplicationData), ExamFileName);
            if (File.Exists(FilePath))
            {
                string[] readContent = ReadAndSplitFile(FilePath);

                if (readContent.Count() > 0)
                {
                    foreach (string line in readContent)
                    {   
                        try
                        {
                            string[] answer = line.Split(':');
                            if (answer.Length > 0)
                            {
                                if (answer[0] == ExamNumber.ToString())
                                {
                                    ItemsExam.Add(Items.FirstOrDefault(x => x.Id == answer[2].ToString()));
                                    //ExamNumber:item.Id_Exam:item.Id:item.Answer_user:item.RadioBtnSelected:item.Answer_clicked:
                                    //item.IsSelected:item.Answer_checked:item.Answer_color

                                    ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Id_Exam = answer[1];

                                    ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Answer_user = answer[3];

                                    ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).RadioBtnSelected = answer[4];
                                    try
                                    {
                                        bool answer_clicked = bool.TryParse(answer[5].ToLower(), out answer_clicked);
                                        ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Answer_clicked =
                                            answer_clicked;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Answer_clicked.(ExamViewModel UserAnswerExam).", ex.Message);
                                    }
                                    try
                                    {
                                        bool isselected = bool.TryParse(answer[6].ToLower(), out isselected);
                                        ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).IsSelected = isselected;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("IsSelected.(ExamViewModel UserAnswerExam).", ex.Message);
                                    }
                                    try
                                    {
                                        bool answer_checked = bool.TryParse(answer[7].ToLower(), out answer_checked);
                                        ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Answer_checked = answer_checked;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("answer_checked.(ExamViewModel UserAnswerExam).", ex.Message);
                                    }
                                    ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Answer_color = answer[8];
                                    ItemsExam.FirstOrDefault(x => x.Id == answer[2].ToString()).Id_ExamNumber = ExamNumber;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Wczytanie poprzednich odpowiedzi(ExamViewModel UserAnswerExam).", ex.Message);
                        }
                    }
                }
            }
        }

    }// class ExamViewModel end

    public class AnswerData
    {
        public int Id { get; set; }
        public int Id_Sorted { get; set; }
        public string AnswerUser { get; set; }
        public string AnswerRight { get; set; }
        public string AnswerClicked { get; set; }
        public string Color { get; set; }
        public string RadioBtnSelected { get; set; }
        public string IsSelected { get; set; }
        public string Answer_checked {  get; set; }
    }
}
