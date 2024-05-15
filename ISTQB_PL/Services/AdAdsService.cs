using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class AdAdsService
    {
        private View AdView { get; set; }

        public View GetAdsGoogleView()
        {
            // Użyj DependencyService, aby uzyskać dostęp do IAdService
            var adService = DependencyService.Get<IAdService>();

            // Sprawdź, czy adService nie jest nullem, co oznacza, że implementacja IAdService została znaleziona
            if (adService != null)
            {
                // Utwórz reklamę
                adService.CreateAd();

                // Dodaanie kodu, aby wyświetlić reklamę na stronie, np. umieszczając ją w odpowiednim miejscu w układzie strony
                AdView = adService.GetAdView();
            }

            return AdView;
        }
    }
}
