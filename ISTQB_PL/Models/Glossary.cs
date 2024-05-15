using ISTQB_PL.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace ISTQB_PL.Models
{
    public class Glossary : BaseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MyFontSize { get; set; }
        public Color MyBackgroundColor { get; set; }
    }
}
