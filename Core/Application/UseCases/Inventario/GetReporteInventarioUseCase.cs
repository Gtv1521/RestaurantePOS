using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Inventario
{
    public class GetReporteInventarioUseCase
    {
        private readonly InventarioRepository _repo;
        public GetReporteInventarioUseCase(InventarioRepository repo) => _repo = repo;

        public Task<InventarioReporteModel?> Execute() => _repo.GetReporteAsync();
    }
}
