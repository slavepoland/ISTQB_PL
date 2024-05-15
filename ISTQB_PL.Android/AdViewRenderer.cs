using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System;
using Android.Gms.Ads.Mediation.CustomEvent;
using Xamarin.Essentials;
using ISTQB_PL.Services;
using ISTQB_PL.Droid;
using static Android.Views.ViewGroup;
using Android.Content;
using ISTQB_PL.Controls;

[assembly: Dependency(typeof(AdService))]
//[assembly: ExportRenderer(typeof(AdControlView), typeof(AdViewRenderer))]
namespace ISTQB_PL.Droid
{
    public class AdService : IAdService
    {
        private AdView MyAdView {  get; set; }

        private readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        //testowa ca-app-pub-3940256099942544/6300978111, moja z admob ca-app-pub-6479256761216523/8121462788
        [Obsolete]
        public void CreateAd()
        {       
            MyAdView = new AdView(Forms.Context)
            {
                AdUnitId = adUnitId //Zastąp prawdziwym identyfikatorem jednostki reklamowej z AdMob
            };

            var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
            if (orientation == DisplayOrientation.Portrait)
            {
                MyAdView.AdSize = AdSize.Banner;
            }
            else
            {
                MyAdView.AdSize = AdSize.FullBanner;
            }

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            MyAdView.LayoutParameters = adParams;

            // Tutaj możesz dostosować inne ustawienia reklamy

            // Dodaj obsługę zdarzenia reklamy pobranej
            MyAdView.AdListener = new AdListenerWrapper(OnAdLoaded, OnAdFailedToLoad);
            var adRequest = new AdRequest.Builder().Build();
            MyAdView.LoadAd(adRequest);
        }

        public View GetAdView()
        {
            if (MyAdView != null)
            {
                return MyAdView.ToView();
            }
            // Zwróć utworzoną reklamę jako View
            return null;
        }

        [Obsolete]
        public void ShowAd()
        {
            if (MyAdView != null)
            {
                // Tutaj dodaj kod do wyświetlania reklamy w odpowiednim miejscu na stronach
            }
        }
        public void AdjustAdSize(bool isPortrait)
        {
            // Pobierz dostęp do widoku reklamy i dostosuj jego rozmiar
            AdView adView = Application.Current.MainPage.FindByName<AdView>("adControlView");

            if (adView != null)
            {
                if (isPortrait)
                {
                    adView.AdSize = AdSize.Banner;
                }
                else
                {
                    adView.AdSize = AdSize.FullBanner;
                }
            }
        }

        // Obsługa zdarzenia reklamy pobranej
        private void OnAdLoaded(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Reklama została pobrana prawidłowo.");
        }

        // Obsługa zdarzenia błędu pobierania reklamy
        private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Błąd pobierania reklamy: {e.ErrorCode}");
        }

        // Klasy podrzędnej implementującej AdListener
        private class AdListenerWrapper : AdListener
        {
            private readonly Action<object, EventArgs> onAdLoaded;
            private readonly Action<object, AdFailedToLoadEventArgs> onAdFailedToLoad;

            public AdListenerWrapper(Action<object, EventArgs> onAdLoaded, Action<object, AdFailedToLoadEventArgs> onAdFailedToLoad)
            {
                this.onAdLoaded = onAdLoaded;
                this.onAdFailedToLoad = onAdFailedToLoad;
            }

            public override void OnAdLoaded()
            {
                onAdLoaded?.Invoke(this, EventArgs.Empty);
            }

            //public override void OnAdFailedToLoad(AdError error)
            //{
            //    onAdFailedToLoad?.Invoke(this, new AdFailedToLoadEventArgs(int.Parse(error.Message)));
            //}
        }
    }

    [System.Obsolete]
    public class AdViewRenderer : ViewRenderer<AdControlView, AdView>
    {
        private readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        //ca-app-pub-3940256099942544/6300978111 - testowe ID
        //ca-app-pub-6479256761216523/8121462788  ca-app-pub-6479256761216523~3144468275 - moje admob

        //Note you may want to adjust this, see further down.
        public AdSize adSize;
        public AdView adView;

        public AdViewRenderer(Context context) : base(context)
        {
        }

        public AdView CreateAdView(Context context)
        {
            if (adView != null)
                return adView;

            adView = new AdView(Forms.Context);

            var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
            if (orientation == DisplayOrientation.Portrait)
            {
                adView.AdSize = AdSize.Banner;
            }
            else
            {
                adView.AdSize = AdSize.FullBanner;
            }
            //adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest.Builder().Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdControlView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                //CreateAdView();
                adView = new AdView(Context)
                {
                    AdSize = AdSize.Banner,
                    AdUnitId = adUnitId  //identyfikator jednostki reklamowej z AdMob
                };
                SetNativeControl(adView);

                // Dodaj obsługę zdarzenia reklamy pobranej
                adView.AdListener = new MyAdListener(OnAdLoaded);

                // Dodaj obsługę zdarzenia zmiany orientacji reklamy
                MessagingCenter.Subscribe<IOrientationHandler, bool>(this, "OrientationChanged", (sender, isPortrait) =>
                {
                    // Tutaj możesz dostosować rozmiar reklamy w zależności od orientacji
                    if (isPortrait)
                    {
                        adView.AdSize = AdSize.Banner;
                    }
                    else
                    {
                        adView.AdSize = AdSize.FullBanner;
                    }
                });
            }
        }

        // Obsługa zdarzenia reklamy pobranej
        private void OnAdLoaded(object sender, EventArgs e)
        {
            // Reklama została pobrana prawidłowo
            System.Diagnostics.Debug.WriteLine("Reklama została pobrana prawidłowo.");
        }

        // Obsługa zdarzenia błędu pobierania reklamy
        private void OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            // Błąd pobierania reklamy, wyświetl szczegóły błędu
            System.Diagnostics.Debug.WriteLine($"Błąd pobierania reklamy: {e.ErrorCode}");
        }

        // Klasy podrzędnej implementującej AdListener
        class MyAdListener : AdListener
        {
            private readonly Action<object, EventArgs> onAdLoaded;
            //private readonly Action<object, AdFailedToLoadEventArgs> onAdFailedToLoad;

            public MyAdListener(Action<object, EventArgs> onAdLoaded)
            {
                this.onAdLoaded = onAdLoaded;
            }

            public override void OnAdLoaded()
            {
                onAdLoaded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}