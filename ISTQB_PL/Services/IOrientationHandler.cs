using System;
using System.Collections.Generic;
using System.Text;

namespace ISTQB_PL.Services
{
    public interface IOrientationHandler
    {
        void NotifyOrientationChanged(bool isPortrait);
    }
}
