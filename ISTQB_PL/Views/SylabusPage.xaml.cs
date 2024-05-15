using ISTQB_PL.ViewModels;
using System;
using System.Collections.Generic;
//using MarcTron.Plugin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ISTQB_PL.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SylabusPage : ContentPage
	{

        private int MyFontSize { get; set; }

        private SylabusViewModel ViewModel { get; set; }
        //private MTAdView AdMobBanner { get; set; }

        public SylabusPage ()
		{
			InitializeComponent ();
            ViewModel = new SylabusViewModel(true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MyFontSize = int.Parse(Application.Current.Properties["FontSize"].ToString());
            var labelsInHierarchy = FindLabelInHierarchy(stackLayout);
            foreach (Label item in labelsInHierarchy)
            {
                item.FontSize = MyFontSize;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Sprawdź, czy bieżąca strona jest typu Shell, a jeśli tak, zamknij ją
            if (Shell.Current != null && Shell.Current.Navigation.NavigationStack.Count == 1)
            {
                Shell.Current.GoToAsync("//AboutPage");
                //Shell.Current.Navigation.PopAsync();
                return true; // Zapobiegnij standardowemu zachowaniu przycisku "Wstecz"
            }
            else
            {
                Shell.Current.GoToAsync("..");
            }
            // Standardowe zachowanie przycisku "Wstecz", jeśli nie jesteśmy w Shell
            return base.OnBackButtonPressed();
        }

        private List<Label> FindLabelInHierarchy(View view)
        {
            List<Label> labels = new List<Label>();

            if (view is Label)
            {
                labels.Add(view as Label);
            }
            else if (view is Grid)
            {
                var grid = view as Grid;
                foreach (var child in grid.Children)
                {
                    labels.AddRange(FindLabelInHierarchy(child));
                }
            }
            else if (view is Frame)
            {
                var frame = view as Frame;
                if (frame.Content is Grid)
                {
                    var contentGrid = frame.Content as Grid;
                    labels.AddRange(FindLabelInHierarchy(contentGrid));
                }
            }
            else if (view is StackLayout)
            {
                var stackLayout = view as StackLayout;
                foreach (var child in stackLayout.Children)
                {
                    labels.AddRange(FindLabelInHierarchy(child));
                }
            }
            return labels;
        }

        private async void OnFrameTapped(object sender, EventArgs e)
        {
            if (sender is Frame tappedFrame)
            {
                string nrRozdzial = (string)((TapGestureRecognizer)tappedFrame.GestureRecognizers[0])
                    .CommandParameter;
                await Navigation.PushAsync(new SylabusChapterPage(ViewModel, nrRozdzial, null));
            }
        }
    }
}