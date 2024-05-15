using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ISTQB_PL.ViewModels
{
    public class ExamPopupViewModel : BaseViewModel
    {
        private bool isOption1Selected;
        public bool IsOption1Selected
        {
            get { return isOption1Selected; }
            set
            {
                if (isOption1Selected != value)
                {
                    if (!isOption1Selected)
                    {
                        Application.Current.Properties["Wersja3"] = "tak";
                    }
                    else
                    {
                        Application.Current.Properties["Wersja3"] = "nie";
                    }
                    isOption1Selected = value;
                    OnPropertyChanged(nameof(IsOption1Selected));
                }
            }
        }

        private bool isOption2Selected;
        public bool IsOption2Selected
        {
            get { return isOption2Selected; }
            set
            {
                if (isOption2Selected != value)
                {
                    if (!isOption2Selected)
                    {
                        Application.Current.Properties["Wersja4"] = "tak";
                    }
                    else
                    {
                        Application.Current.Properties["Wersja4"] = "nie";
                    } 
                    isOption2Selected = value;
                    OnPropertyChanged(nameof(IsOption2Selected));
                }
            }
        }

        public ICommand DismissCommand { get; private set; }
        private readonly TaskCompletionSource<bool> popupClosedTaskCompletionSource;

        public ExamPopupViewModel()
        {
            IsOption1Selected = false;
            IsOption2Selected = false;

            DismissCommand = new Command(DismissPopup);
            popupClosedTaskCompletionSource = new TaskCompletionSource<bool>();
        }

        public async Task WaitForPopupCloseAsync()
        {
            await popupClosedTaskCompletionSource.Task;
        }

        private void DismissPopup()
        {
            // Tutaj możesz przekazać wartość IsOptionSelected lub inny stan do głównej strony lub gdziekolwiek go potrzebujesz.
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
            popupClosedTaskCompletionSource.SetResult(true);
        }
    }
}
