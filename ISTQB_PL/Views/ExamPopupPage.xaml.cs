using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExamPopupPage : Rg.Plugins.Popup.Pages.PopupPage
	{
        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public ExamPopupPage ()
		{
			InitializeComponent ();
            MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Application.Current.Properties.ContainsKey("Wersja3"))
            {
                switch (Application.Current.Properties["Wersja3"].ToString())
                {
                    case "tak": VersionThreeCheckBox.IsChecked = true; break;
                    case "nie": VersionThreeCheckBox.IsChecked = false; break;
                }
            }
            else
            {
                Application.Current.Properties["Wersja3"] = "nie";
            }

            if (Application.Current.Properties.ContainsKey("Wersja4"))
            {
                switch(Application.Current.Properties["Wersja4"].ToString())
                {
                    case "tak": VersionFourCheckBox.IsChecked = true;  break;
                    case "nie": VersionFourCheckBox.IsChecked = false; break;
                }
            }
            else
            {
                Application.Current.Properties["Wersja4"] = "nie";
            }

            VersionThreeCheckBox.Color = MainTextColor;
            VersionFourCheckBox.Color = MainTextColor;
            VersionThreeLabel.TextColor = MainTextColor;
            VersionFourLabel.TextColor = MainTextColor;
            stackLayout.BackgroundColor = MainBackgroundColor;
            activityIndycator.Color = MainTextColor;
            LabelText.TextColor = MainTextColor;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (sender is Label label)
            {
                switch (label.ClassId)
                {
                    case "3":
                    {
                        switch (VersionThreeCheckBox.IsChecked)
                        {
                            case true: VersionThreeCheckBox.IsChecked = false; break;
                            case false: VersionThreeCheckBox.IsChecked = true; break;
                        }
                        break;
                    }
                    case "4":
                    {
                        switch (VersionFourCheckBox.IsChecked)
                        {
                            case true: VersionFourCheckBox.IsChecked = false; break;
                            case false: VersionFourCheckBox.IsChecked = true; break;
                        }
                        break;
                    }
                }
            }
        }
    }
}