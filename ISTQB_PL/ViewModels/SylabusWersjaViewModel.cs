using ISTQB_PL.Models;
using ISTQB_PL.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class SylabusWersjaViewModel : BaseViewModel
    {
        private string id;
        private string sylabus;
        private string jezyk;
        private string wersja;

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        public string Sylabus
        {
            get => sylabus;
            set => SetProperty(ref sylabus, value);
        }
        public string Jezyk
        {
            get => jezyk;
            set => SetProperty(ref jezyk, value);
        }
        public string Wersja
        {
            get => wersja;
            set => SetProperty(ref wersja, value);
        }

        public Command LoadItemsCommand { get; }
        public ObservableCollection<SylabusWersja> Items { get; set; }

        public SylabusWersjaViewModel()
        {
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            Items = new ObservableCollection<SylabusWersja>();
            _ = ExecuteLoadItemsCommand();
        }

        public async Task<IEnumerable<SylabusWersja>> ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await SylabusWersjaStore.GoogleSheetsSylabusWersja();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
            finally
            {
                IsBusy = false;
            }

            return await Task.FromResult(Items);
        }
    }
}
