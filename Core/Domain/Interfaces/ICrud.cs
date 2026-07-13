using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Interfaces
{
    public interface ICrud<T, R>
    {
        Task<T> GetAsync(string id);
        Task<string?> CreateAsync(R data);
        Task<bool> UpdateAsync(string id, R data);
        Task<bool> DeleteAsync(string id);
    }

    public interface ISingleCrud<T, R> : ICrud<T, R>
    {
        Task<IEnumerable<T>> GetAllAsync(string id, int? page, int? size);
    }

    public interface IMultipleCrud<T, R> : ICrud<T, R>
    {
        Task<IEnumerable<T>> GetAllAsync();
    }

    public interface IGetList<T>
    {
        Task<IEnumerable<T>> GetAllDataAsync(string id);
    }
}