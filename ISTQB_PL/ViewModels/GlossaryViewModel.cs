using ISTQB_PL.Models;
using ISTQB_PL.Services;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using System.Linq;
using System.Reflection;

namespace ISTQB_PL.ViewModels
{
    public class GlossaryViewModel : BaseViewModel
    {
        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }
        }

        private Color _myBackGroundColor;
        public Color MyBackgroundColor
        {
            get { return _myBackGroundColor; }
            set
            {
                if (_myBackGroundColor != value)
                {
                    _myBackGroundColor = value;
                    OnPropertyChanged(nameof(MyBackgroundColor));
                }
            }
        }

        private Color _myFontColor;
        public Color MyFontColor
        {
            get { return _myFontColor; }
            set
            {
                if (_myFontColor != value)
                {
                    _myFontColor = value;
                    OnPropertyChanged(nameof(MyFontColor));
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));

                    // Wywołaj metodę do filtrowania po każdej zmianie tekstu
                    FilterItems();
                }
            }
        }

        private bool _startsWithSearchMode = true;
        public bool StartsWithSearchMode
        {
            get { return _startsWithSearchMode; }
            set
            {
                if (_startsWithSearchMode != value)
                {
                    _startsWithSearchMode = value;
                    OnPropertyChanged(nameof(StartsWithSearchMode));
                    UpdateLabelText();
                    FilterItems();
                }
            }
        }

        private string _labelText;
        public string LabelText
        {
            get { return _labelText; }
            set
            {
                if (_labelText != value)
                {
                    _labelText = value;
                    OnPropertyChanged(nameof(LabelText));
                    UpdateLabelText();
                }
            }
        }


        public ICommand SearchCommand { get; }

        public ObservableCollection<Glossary> SearchItems { get; private set; }
        public ObservableCollection<Glossary> Items { get; private set; }

        public GlossaryViewModel()
        {
            _fontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            _myBackGroundColor = (Color)Application.Current.Resources["JasneTlo"];
            _myFontColor = (Color)Application.Current.Resources["JasnyTekst"];
            Title = "Glosariusz";
            Items = new ObservableCollection<Glossary>();
            SearchItems = new ObservableCollection<Glossary>();

            _ = ExecuteLoadItemsCommand();

            SearchCommand = new Command(ExecuteSearch);

            UpdateLabelText();
        }

        public async Task<IEnumerable<Glossary>> ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await GlossaryStore.GoogleSheetsGlossary();
                foreach (var item in items)
                {
                    item.MyFontSize = FontSize;
                    Items.Add(item);
                    SearchItems.Add(item);
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

            return await Task.FromResult(Items);
        }

        private void ExecuteSearch()
        {
            // Tutaj możesz obsłużyć logikę wyszukiwania
            FilterItems();
        }

        private void FilterItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Jeśli pole wyszukiwania jest puste, przywróć oryginalną kolekcję
                SearchItems.Clear();
                foreach (var item in Items)
                {
                    SearchItems.Add(item);
                }
            }
            else
            {
                // Filtruj kolekcję na podstawie wprowadzonego tekstu i trybu wyszukiwania
                var filteredItems = Items
                    .Where(item =>
                        _startsWithSearchMode ? item.Name.StartsWith(SearchText, StringComparison.OrdinalIgnoreCase) :
                                               item.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Aktualizuj kolekcję
                SearchItems.Clear();
                foreach (var item in filteredItems)
                {
                    SearchItems.Add(item);
                }
            }
        }

        public string UpdateLabelText()
        {
            return LabelText = StartsWithSearchMode ? "Szukaj od początku tekstu" : "Szukaj słowo zawierające tekst";
        }
    }
}
