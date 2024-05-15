using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyPopupPage : Rg.Plugins.Popup.Pages.PopupPage
	{
        Color MainTextColor { get; set; }
        Color MainBackgroundColor { get; set; }

        public MyPopupPage ()
		{
			InitializeComponent ();
            MainTextColor = (Color)Application.Current.Resources["JasnyTekst"];
            MainBackgroundColor = (Color)Application.Current.Resources["JasneTlo"];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            stackLayout.BackgroundColor = MainBackgroundColor;
            activityIndycator.Color = MainTextColor;
            LabelText.TextColor = MainTextColor;
        }
    }
}