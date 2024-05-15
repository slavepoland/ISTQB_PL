using ISTQB_PL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Controls
{
    public class AdControlView : View
    {
        // W tej metodzie możesz powiadomić o zmianie orientacji
        public void OnOrientationChanged(bool isPortrait)
        {
            (Application.Current as IOrientationHandler)?.NotifyOrientationChanged(isPortrait);
        }
    }
}
