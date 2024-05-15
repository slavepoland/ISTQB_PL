using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using ISTQB_PL.Services;
using Xamarin.Forms;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Content.Res;
using AndroidX.Core.Content;
using static ISTQB_PL.Droid.MainActivity;
//using Android.Media.TV;
//using Android.Gms.Ads;
//using AdRequest = Android.Gms.Ads.AdRequest;

//[assembly: Dependency(typeof(FileReadWrite))]
//[assembly: Dependency(typeof(FileRead))]
[assembly: Dependency(typeof(NavigationService))]
[assembly: ExportRenderer(typeof(MyRadioButton), typeof(MyRadioButtonRenderer))]
namespace ISTQB_PL.Droid
{
    [Activity(Label = "Tester Manualny PL", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            Rg.Plugins.Popup.Popup.Init(this); // Initialize Rg.Plugins.Popup
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class NavigationService : INavigationService
        {
            public void GoBack()
            {
                var currentPage = GetCurrentPage();

                if (currentPage != null && currentPage.Navigation != null)
                {
                    // Sprawdź, czy bieżąca strona jest typu Shell, a jeśli tak, zamknij ją
                    if (Shell.Current != null && Shell.Current.Navigation.NavigationStack.Count == 1)
                    {
                        Shell.Current.GoToAsync("//AboutPage");
                    }
                    else
                    {
                        Shell.Current.GoToAsync("..");
                    }
                    //currentPage.Navigation.PopAsync(); // Zamykanie aktualnej strony
                }
            }

            private Page GetCurrentPage()
            {
                return Xamarin.Forms.Application.Current.MainPage;
            }
        }

        public class MyRadioButtonRenderer : RadioButtonRenderer, ICustomRadioButtonRenderer
        {
            public MyRadioButtonRenderer(Context context) : base(context)
            {
            }

            protected override void OnElementChanged(ElementChangedEventArgs<RadioButton> e)
            {
                base.OnElementChanged(e);

                if (e.NewElement != null)
                {
                    UpdateRadioButtonColor((MyRadioButton)e.NewElement);
                }
            }

            public void UpdateRadioButtonColor(MyRadioButton radioButton)
            {
                int checkedColor;
                if (App.Current.RequestedTheme == OSAppTheme.Dark)
                {
                    checkedColor = ContextCompat.GetColor(Context, Resource.Color.checked_color_dark_mode); // Kolor dla trybu ciemnego, gdy RadioButton jest zaznaczony
                }
                else
                {
                    checkedColor = ContextCompat.GetColor(Context, Resource.Color.checked_color_light_mode); // Kolor dla trybu jasnego, gdy RadioButton jest zaznaczony
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    // Dostępne tylko dla Androida 10 (API poziom 29) i nowszych
                    Control.ButtonTintList = ColorStateList.ValueOf(new Android.Graphics.Color(checkedColor));
                }
            }
        }


        //public class FileReadWrite : IFileReadWrite
        //{
        //    public async Task WriteTextAsync(string filename, string text)
        //    {
        //        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        //        string filePath = Path.Combine(documentsPath, filename);

        //        using (StreamWriter writer = File.CreateText(filePath))
        //        {
        //            await writer.WriteAsync(text);
        //        }
        //    }
        //}
        //public class FileRead : IFileRead
        //{
        //    public async Task<string> ReadTextAsync(string filename)
        //    {
        //        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        //        string filePath = Path.Combine(documentsPath, filename);

        //        if (File.Exists(filePath))
        //        {
        //            using (StreamReader reader = File.OpenText(filePath))
        //            {
        //                return await reader.ReadToEndAsync();
        //            }
        //        }
        //        else
        //        {
        //            return null; // File does not exist
        //        }
        //    }
        //}
    }
}