using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ISTQB_PL.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(AdMobView), typeof(ISTQB_PL.Droid.AdMobViewRenderer))]
namespace ISTQB_PL.Droid
{
    [Obsolete]
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        private readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        //ca-app-pub-6479256761216523/8121462788  ca-app-pub-6479256761216523~3144468275 - moje admob

        //ca-app-pub-3940256099942544/6300978111 

        //ca-app-pub-2457246758474079/2265487462 //ca-app-pub-2457246758474079~4368002667
        //Note you may want to adjust this, see further down.
        readonly AdSize adSize = AdSize.Banner;
        AdView adView;


        public AdMobViewRenderer(Context context) : base(context)
        {
        }

        AdView CreateAdView()
        {
            if (adView != null)
                return adView;

            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it

            adView = new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest.Builder().Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Tutaj dodaj kod do utworzenia i skonfigurowania reklamy AdMob
                // Użyj AdView z pakietu Xamarin.GooglePlayServices.Ads
                CreateAdView();
                SetNativeControl(adView);
            }
        }
    }
}