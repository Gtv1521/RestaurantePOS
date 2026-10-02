using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Venta
{
    public class CrearComandaUseCase
    {
        private readonly ComandaRepository _repo;

        public CrearComandaUseCase(ComandaRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> Execute(int ventaId, int meseroId, List<ComandaItemRequest> items)
        {
            return _repo.CrearAsync(new ComandaCreateRequest
            {
                VentaId = ventaId,
                MeseroId = meseroId,
                Items = items
            });
        }
    }
}
