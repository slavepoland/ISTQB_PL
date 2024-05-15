using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISTQB_PL.Droid.Helpers;
using Xamarin.Forms.Platform.Android;
using Android.Hardware.Camera2;
using Android.Gms.Ads;
using Xamarin.Forms;
using ISTQB_PL.Controls;

namespace ISTQB_PL.Droid.Helpers
{
    [assembly: ExportRenderer(typeof(AdControlView))
     typeof(AdViewRenderer))]
    [Obsolete]
    public class AdViewRenderer : ViewRenderer<Controls.AdControlView, AdView>
    {
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        AdView CreateAdView()
        {
            if(adView != null)
                return adView;

            adView = new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId; 

            var adParams = new LinearLayout.LayoutParams(
                LayoutParams.WrapContent, LayoutParams.WrapContent );

            adView.LayoutParameters = adParams;

            adView.LoadAd(new AdRequest.Builder().Build());

            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdControlView> e)
        {
            base.OnElementChanged(e);

            if(Control != null)
            {
                CreateAdView();
                SetNativeControl(adView);
            }
        }
    }
}