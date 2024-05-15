using ISTQB_PL.Services;
using Java.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private int selectedOption;
        //private readonly string filename = "fontsize.txt";

        public int SelectedOption
        {
            get => selectedOption;
            set
            {
                selectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
                OnPropertyChanged(nameof(SelectedOptionText));
                OnPropertyChanged(nameof(SelectedOptionFontSize));
            }
        }

        public double SelectedOptionFontSize
        {
            get
            {
                // Define font sizes for each option
                return selectedOption switch
                {
                    0 => 12,// Font size for Option 1
                    1 => 13,// Font size for Option 2
                    2 => 14,// Font size for Option 3
                    3 => 15,// Font size for Option 4
                    4 => 16,// Font size for Option 5
                    5 => 17,
                    6 => 18,
                    7 => 19,
                    8 => 20,
                    9 => 21,
                    10 => 22,
                    11 => 23,
                    12 => 24,
                    13 => 25,
                    14 => 26,
                    15 => 27,
                    16 => 28,
                    _ => (double)16// Default font size
                };
            }
        }

        public string SelectedOptionText
        {
            get
            {
                return $"Tak będzie wyglądał główny tekst. {SelectedOptionFontSize} DPI";
            }
        }

        public ICommand OpenBrowserCommand { get; private set; }

        public SettingsViewModel()
        {
            // Inicjalizacja komendy
            OpenBrowserCommand = new Command<string>(OpenBrowser);
        }

        //public int GetFontSizeFromFile ()
        //{
        //    int fontOption = 4; //domyślny rozmiar czcionki
        //    try
        //    {
        //        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        //        string fileContent = File.ReadAllText(filePath);

        //        string[] parts = fileContent.Split(':');

        //        if(int.TryParse(parts[1], out fontOption))
        //        {
        //            return fontOption;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    return fontOption;
        //}

        private async void OpenBrowser(string url)
        {
            Uri uri = new Uri($"{url}");
            await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
    }
        
//namespace
}

