using ISTQB_PL.Models;
using ISTQB_PL.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private Item _selectedItem;
        private string itemId;
        private string mycontent;
        private string answer_a;
        private string answer_b;
        private string answer_c;
        private string answer_d;
        private string answer_right;
        private string str_picture;
        private string str_explanation;
        private string exp_picture;
        private string answer_user;
        private bool visible_picture;

        public string Id { get; set; }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }
        public string MyContent
        {
            get => mycontent;
            set => SetProperty(ref mycontent, value);
        }
        public string Answer_a
        {
            get => answer_a;
            set => SetProperty(ref answer_a, value);
        }
        public string Answer_b
        {
            get => answer_b;
            set => SetProperty(ref answer_b, value);
        }
        public string Answer_c
        {
            get => answer_c;
            set => SetProperty(ref answer_c, value);
        }
        public string Answer_d
        {
            get => answer_d;
            set => SetProperty(ref answer_d, value);
        }
        public string Answer_Right
        {
            get => answer_right;
            set => SetProperty(ref answer_right, value);
        }
        public string Str_Picture
        {
            get => str_picture;
            set => SetProperty(ref str_picture, value);
        }
        public string Str_Explanation
        {
            get => str_explanation;
            set => SetProperty(ref str_explanation, value);
        }
        public string Exp_Picture
        {
            get => exp_picture;
            set => SetProperty(ref exp_picture, value);
        }
        public string Answer_User
        {
            get => answer_user;
            set => SetProperty(ref answer_user, value);
        }
        public bool Visible_Picture
        {
            get => visible_picture;
            set => SetProperty(ref visible_picture, value);
        }

        //public ICommand CheckAnswerCommand { get; }
        public ObservableCollection<Item> Items { get; set; }

        public ItemDetailViewModel()
        {
            //CheckAnswerCommand = new Command(async () => await ExecuteCheckAnswerCommand());
            Title = "Pytanie";
            Items = new ObservableCollection<Item>();
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        //public async Task ExecuteCheckAnswerCommand()
        //{
        //    IsBusy = true;

        //    try
        //    {
        //        //var items = await DataStore.GoogleSheetsConnect(true);
        //        //foreach (var item in items)
        //        //{
        //        //    Items.Add(item);
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                MyContent = item.MyContent;
                Answer_a = item.Answer_a;
                Answer_b = item.Answer_b;
                Answer_c = item.Answer_c;
                Answer_d = item.Answer_d;
                Answer_Right = item.Answer_right;
                Answer_User = item.Answer_user;
                Str_Picture = item.Str_Picture;
                Exp_Picture = item.Exp_Picture;
                Str_Explanation = item.Str_Explanation;
                if (item.Str_Picture != "null")
                {
                    Visible_Picture = true;
                }
                else
                {
                    Visible_Picture = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var alertService = DependencyService.Get<IAlertService>();
                await alertService.DisplayAlert("Title", ex.ToString(), "OK");
            }
        }
    }
}
