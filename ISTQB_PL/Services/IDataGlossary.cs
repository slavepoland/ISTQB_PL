using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IDataGlossary<T>
    {
        Task<IEnumerable<T>> GoogleSheetsGlossary();
    }
}
