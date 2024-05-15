using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IAlertService
    {
        Task DisplayAlert(string title, string message, string cancel);
        Task<bool> DisplayAlertCommit(string title, string message, string commit, string cancel);
    }
}
