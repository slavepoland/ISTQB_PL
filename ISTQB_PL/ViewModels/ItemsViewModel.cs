using ISTQB_PL.Models;
using ISTQB_PL.Services;
using ISTQB_PL.Views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace ISTQB_PL.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    MessagingCenter.Send(this, "ItemSelected", _selectedItem);
                }
            }
        }

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command LoadFreshItemsCommand { get; }
        public Command GoBackItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Manual tester - pytania";
            //Title = title;
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            LoadFreshItemsCommand = new Command(async () => await ExecuteFreshLoadItemsCommand());

            //ItemTapped = new Command<Item>(OnItemSelected);

            GoBackItemsCommand = new Command(async () => await Shell.Current.GoToAsync("//AboutPage"));

            _ = ExecuteFreshLoadItemsCommand();
        }


        async Task ExecuteFreshLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                Items.Clear();
                var items = await DataStore.GoogleSheetsExam(true);
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
            await PopupNavigation.Instance.PopAsync();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                if (Items.Count == 0)
                {
                    Items.Clear();
                    var items = await DataStore.GoogleSheetsExam();
                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
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

        public void OnAppearing()
        {
            IsBusy = true;
            //SelectedItem = null;
        }

        

        //private async void GoBack()
        //{
        //    // This will pop the current page off the navigation stack
        //    await Shell.Current.GoToAsync("..");
        //}

        //async void OnItemSelected(Item item)
        //{
        //    if (item == null)
        //        return;

        //    // This will push the ItemDetailPage onto the navigation stack
        //    await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        //}
    }
}