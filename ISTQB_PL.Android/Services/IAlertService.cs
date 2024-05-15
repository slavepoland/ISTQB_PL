using System.Threading.Tasks;

namespace ISTQB_PL.Droid.Services
{
    public interface IAlertService
    {
        Task DisplayAlert(string title, string message, string cancel);
    }
}
