using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Interfaces
{
    public interface ICrud<T, R>
    {
        Task<T> GetAsync(int id);
        Task<int?> CreateAsync(R data);
        Task<bool> UpdateAsync(int id, R data);
        Task<bool> DeleteAsync(int id);
    }

    public interface ISingleCrud<T, R> : ICrud<T, R>
    {
        Task<IEnumerable<T>> GetAllAsync(int id, int? page, int? size);
    }

    public interface IMultipleCrud<T, R> : ICrud<T, R>
    {
        Task<IEnumerable<T>> GetAllAsync();
    }

    public interface IGetList<T>
    {
        Task<IEnumerable<T>> GetAllDataAsync(int id);
    }
    public interface IGetOpens<T>
    {
        Task<IEnumerable<T>> TablesOpen();
    }

    public interface IOptionsMesas<T>
    {
        Task<T> OcuparMesa(int id);
        Task<bool> LiberarMesa(int id);
        Task<bool> Reservar(int id, string? nota = null);
    }

    public interface IOptionVenta
    {
        Task<bool> UptatePax(int id, int pax);
    }

}