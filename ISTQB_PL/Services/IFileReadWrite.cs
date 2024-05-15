using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IFileReadWrite
    {
        Task WriteTextAsync(string filename, string text);
    }
}
