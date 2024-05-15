using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public interface IAdService
    {
        void AdjustAdSize(bool isPortrait);
        void ShowAd();
        void CreateAd();
        View GetAdView();
    }
}
