using ISTQB_PL.Services;
using ISTQB_PL.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WhatIsISTQB : ContentPage
	{
		public WhatIsISTQB ()
		{
			InitializeComponent ();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
        }

        protected override void OnAppearing()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                base.OnAppearing();
                WhatisISTQBLabelText();
            }
        }

		private async void WhatisISTQBLabelText()
		{
            var content = new SylabusViewModel(false);
            LabelContent.Text = await content.WhatIsISTQB();
        }
    }
}