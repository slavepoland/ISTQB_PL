using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IDataSylabus<T>
    {
        Task<IEnumerable<T>> GoogleSheetsSylabus(bool forceRefresh = false);
        Task<string> GoogleSheetsWhatIsSylabus();
    }
}
