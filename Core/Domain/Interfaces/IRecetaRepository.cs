using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using RestaurantePOS.Core.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Domain.Interfaces
{
    public interface IRecetaRepository: ICrud<RecetaModel, RecetaRequest>
    {
        Task<List<RecetaRequest>> GetByProductoIdAsync(int productId);
        Task UpdateAsync(int productId, List<RecetaRequest> receta);
    }
}
