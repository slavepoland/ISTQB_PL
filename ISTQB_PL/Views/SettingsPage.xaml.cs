using ISTQB_PL.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.App.Assist.AssistStructure;

namespace ISTQB_PL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private int selectedOption;
        public string SelectedOptionText
        {
            get
            {
                return $"Tak będzie wyglądał główny tekst - {SelectedOptionFontSize} DPI";
            }
        }

        private int SelectedOptionFontSize
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
                    5 => 17,//...
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
                    _ => 16// Default font size
                };
            }
        }

        public SettingsPage()
        {
            InitializeComponent();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                // Brak dostępu do Internetu
                DisplayAlert("Brak dostępu do Internetu", "Aplikacja nie ma dostępu do Internetu.", "OK");
            }
            MyOptionSlider.ValueChanged += OnSliderValueChanged;
            MyLblExampleText.Text = SelectedOptionText;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            OdpSwitch.IsToggled = Application.Current.Properties.ContainsKey("OdpSwitch")
            && (bool)Application.Current.Properties["OdpSwitch"];

            ExpSwitch.IsToggled = Application.Current.Properties.ContainsKey("ExpSwitch")
            && (bool)Application.Current.Properties["ExpSwitch"];

            MyOptionSlider.Value = Application.Current.Properties.ContainsKey("SliderValue") ?
                int.Parse(Application.Current.Properties["SliderValue"].ToString()) : 4;
            _ = Application.Current.SavePropertiesAsync();

            ChangeFontSizeInHierarchy();
        }

        private void ChangeFontSizeInHierarchy()
        {
            var labelsInFrame = FindLabelInHierarchy(stackLayout);
            foreach (Label item in labelsInFrame)
            {
                item.FontSize = SelectedOptionFontSize;
            }
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

        private async void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            MyOptionSlider.Value = selectedOption = (int)e.NewValue;

            //MyLblExampleText.FontSize = SelectedOptionFontSize;
            MyLblExampleText.Text = SelectedOptionText;
            ChangeFontSizeInHierarchy();

            Application.Current.Properties["SliderValue"] = (int)e.NewValue;
            Application.Current.Properties["FontSize"] = SelectedOptionFontSize;
            await Application.Current.SavePropertiesAsync();
        }

        private async void SwitchToggled(object sender, ToggledEventArgs e)
        {
            bool isToggled = e.Value;
            Switch @switch = sender as Switch;
            if (@switch.TabIndex == 1)
            {
                LblStatus.Text = isToggled ? "Po zakończeniu" : "Natychmiast";
                Application.Current.Properties["OdpSwitch"] = e.Value;
            }
            else
            {
                ExpStatus.Text = isToggled ? "Zawsze" : "Tylko błędne";
                Application.Current.Properties["ExpSwitch"] = e.Value;
            }
            await Application.Current.SavePropertiesAsync();
        }

        private async void OnFrameTapped(object sender, EventArgs e)
        {
            if (sender is Frame)
            {
                // Pokaż Popup z aktywatorem
                var popup = new MyPopupPage();
                await PopupNavigation.Instance.PushAsync(popup);
                await Navigation.PushAsync(new WhatIsISTQB());
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                    await PopupNavigation.Instance.PopAsync();
            }
        }
    }
    //Namespace
}
