using ISTQB_PL.Models;
using ISTQB_PL.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class SylabusViewModel : BaseViewModel
    {
        //private string id;
        //private string rozdzial;
        //private string rozdzial_description;
        //private string podrozdzial;
        //private string podrozdzial_description;
        //private string podpodrozdzial;
        //private string podpodrozdzial_description;
        //private string content;
        //private string umiejetnosci;

        //public string Id
        //{
        //    get => id;
        //    set => SetProperty(ref id, value);
        //}
        //public string Rozdzial
        //{
        //    get => rozdzial;
        //    set => SetProperty(ref rozdzial, value);
        //}
        //public string Rozdzial_description
        //{
        //    get => rozdzial_description;
        //    set => SetProperty(ref rozdzial_description, value);
        //}
        //public string Podrozdzial
        //{
        //    get => podrozdzial;
        //    set => SetProperty(ref podrozdzial, value);
        //}
        //public string Podrozdzial_description
        //{
        //    get => podrozdzial_description;
        //    set => SetProperty(ref podrozdzial_description, value);
        //}
        //public string Podpodrozdzial
        //{
        //    get => podpodrozdzial;
        //    set => SetProperty(ref podpodrozdzial, value);
        //}
        //public string Podpodrozdzial_description
        //{
        //    get => podpodrozdzial_description;
        //    set => SetProperty(ref podpodrozdzial_description, value);
        //}
        //public string Content
        //{
        //    get => content;
        //    set => SetProperty(ref content, value);
        //}
        //public string Umiejetnosci
        //{
        //    get => umiejetnosci;
        //    set => SetProperty(ref umiejetnosci, value);
        //}
        public Command LoadItemsCommand { get; }
        public ObservableCollection<Sylabus> Items { get; set; }
        public string WhatIsISTQBText {  get; set; }



        public SylabusViewModel(bool choice)
        {  
            if(choice)
            {
                LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
                Title = "Pytanie";
                Items = new ObservableCollection<Sylabus>();
                _ = ExecuteLoadItemsCommand();
            }
            else
            {
                _ = WhatIsISTQB();
            }
            
        }

        public async Task<string> WhatIsISTQB()
        {
            IsBusy = true;
            try
            {
                WhatIsISTQBText = await SylabusStore.GoogleSheetsWhatIsSylabus();
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

            return await Task.FromResult(WhatIsISTQBText);
        }

        public async Task<IEnumerable<Sylabus>> ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var items = await SylabusStore.GoogleSheetsSylabus();
                foreach (var item in items)
                {
                    Items.Add(item);
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
    }
}
