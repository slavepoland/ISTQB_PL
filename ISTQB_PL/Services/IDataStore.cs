using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISTQB_PL.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        Task<IEnumerable<T>> GoogleSheetsExam(bool forceRefresh = false);
        Task<bool> GoogleSheetsLogin(string login, string passwd);
        Task<bool> GoogleLastLogin(DateTime actualdate, string login);
    }
}
