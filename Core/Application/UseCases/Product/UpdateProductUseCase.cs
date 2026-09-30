using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Product
{
    public class UpdateProductUseCase
    {
        private readonly IMultipleCrud<ProductoModel, ProductoRequest> _repo;

        public UpdateProductUseCase(IMultipleCrud<ProductoModel, ProductoRequest> repo)
        {
            _repo = repo;
        }

        public async Task<bool> Execute(string id, ProductoRequest request)
        {
            return await _repo.UpdateAsync(id, request);
        }
    }
}
