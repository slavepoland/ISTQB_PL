using ISTQB_PL.ViewModels;
using System.Linq;
using System;
using Xamarin.Forms;


namespace ISTQB_PL.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        private readonly ItemDetailViewModel ViewModel;

        private int MyFontSize
        {
            get => myFontSize;
            set => myFontSize = value;
        }
        private int myFontSize;

        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new ItemDetailViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LabelId.Text = ViewModel.Id + ". Pytanie:";
            ViewModel.OnAppearing();

            if (Application.Current.Properties.ContainsKey("FontSize"))
            {
                MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            }
            else
            {
                MyFontSize = 16;
            }

            LabelId.FontSize = MyFontSize;
            LabelContent.FontSize = MyFontSize;
            foreach(var radioButton in GridAnswer.Children)
            {
                if(radioButton is RadioButton)
                {
                    RadioButton radioButton2 = radioButton as RadioButton;
                    radioButton2.FontSize = MyFontSize;
                }  
            }
            LabelExplanation.FontSize = MyFontSize;
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            GridDefaultRadioButtonBorderColor();

            RadioButton radioButton = sender as RadioButton;

            ViewModel.Answer_User = radioButton.Value as string; //add user_answer to binding

            radioButton.BorderColor = Color.Orange; //set color BorderColor - checked answer

            BtnAnswer.IsVisible = true;  
        }

        private void BtnAnswer_Clicked(object sender, System.EventArgs e)
        {
            if(ViewModel.Answer_User.ToLower() == ViewModel.Answer_Right.ToLower())
            {
                AnswerRight();
            }
            else
            {
                AnswerWrong();
            }
        }

        //function
        void OnSwiped(object sender, SwipedEventArgs e)
        {
            //var item = viewModel.Items.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            //int idexam = int.Parse(item.Id_Exam);
            //switch (e.Direction)
            //{
            //    case SwipeDirection.Left:
            //        if (idexam < 40)
            //        {
            //            idexam += 1;
            //            ItemsListView.SelectedItem = viewModel.Items[idexam - 1];
            //        }
            //        break;
            //    case SwipeDirection.Right:
            //        if (idexam > 1)
            //        {
            //            idexam -= 1;
            //            ItemsListView.SelectedItem = viewModel.Items[idexam - 1];
            //        }
            //        break;
            //}
        }


        private void AnswerRight()
        {
            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is RadioButton)
                {
                    RadioButton radio = grid as RadioButton;
                    if (radio.Value.ToString() == ViewModel.Answer_Right.ToString())
                    {
                        radio.BorderColor = Color.Green;
                        BtnAnswer.ImageSource = "ic_launcher_good.png";
                        break;
                    }
                }
            }
        }

        private void AnswerWrong()
        {
            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is RadioButton)
                {
                    RadioButton radio = grid as RadioButton;
                    if (radio.Value.ToString() == ViewModel.Answer_User.ToString())
                    {
                        radio.BorderColor = Color.Red;
                    }
                    if (radio.Value.ToString() == ViewModel.Answer_Right.ToString())
                    {
                        radio.BorderColor = Color.Green;
                    }
                }
            }
            BtnAnswer.ImageSource = "ic_launcher_wrong.png";
            FrameExplanation.IsVisible = true;
            LabelExplanation.IsVisible = true;
        }

        private void GridDefaultRadioButtonBorderColor()
        {
            foreach (var grid in GridAnswer.Children) //set radiobutton BorderColor - default
            {
                if (grid is RadioButton)
                {
                    RadioButton radio = grid as RadioButton;
                    radio.BorderColor = Color.FromHex("#2196F3");
                    //BtnAnswer.ImageSource = "ic_launcher_questions.png";
                }
            }
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            //int nrQuestions = int.Parse(ViewModel.Id);
            //BindingContext = viewModel = new ItemDetailViewModel();
            //var item = viewModel.Items.FirstOrDefault(x => x.Id == CurrentSelectionItemsListView); //CurrentSelectionItemsListView
            ////int idexam = int.Parse(item.Id_Exam);

            //ImageButton imageButton = (ImageButton)sender;
            //if (imageButton.TabIndex == 1)
            //{
            //    if (idexam > 1)
            //    {
            //        idexam -= 1;
            //        //ItemsListView.SelectedItem = viewModel.Items[idexam - 1];
            //    }
            //}
            //else
            //{
            //    if (idexam < 40)
            //    {
            //        idexam += 1;
            //        //ItemsListView.SelectedItem = viewModel.Items[idexam - 1];
            //    }
            //}
        }

    }
}
