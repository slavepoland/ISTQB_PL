using Foundation;
using ISTQB_PL.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using static ISTQB_PL.iOS.AppDelegate;

[assembly: Dependency(typeof(FileRead))]
[assembly: Dependency(typeof(FileReadWrite))]
namespace ISTQB_PL.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Rg.Plugins.Popup.Popup.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public class FileReadWrite : IFileReadWrite
        {
            public async Task WriteTextAsync(string filename, string text)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filePath = Path.Combine(documentsPath, filename);

                using (StreamWriter writer = File.CreateText(filePath))
                {
                    await writer.WriteAsync(text);
                }
            }
        }
        public class FileRead : IFileRead
        {
            public async Task<string> ReadTextAsync(string filename)
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filePath = Path.Combine(documentsPath, filename);

                if (File.Exists(filePath))
                {
                    using (StreamReader reader = File.OpenText(filePath))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    return null; // File does not exist
                }
            }
        }
    }
}
