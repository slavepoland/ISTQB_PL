using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IFileRead
    {
        Task<string> ReadTextAsync(string filename);
    }
}
