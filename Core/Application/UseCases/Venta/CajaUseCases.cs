using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Venta
{
    public class GetVentasActivasUseCase
    {
        private readonly IOptionVenta _repo;
        public GetVentasActivasUseCase(IOptionVenta repo) => _repo = repo;

        public Task<IEnumerable<VentaActivaModel>> Execute() => _repo.GetActivasAsync();
    }

    public class GetVentaDetalleUseCase
    {
        private readonly IOptionVenta _repo;
        public GetVentaDetalleUseCase(IOptionVenta repo) => _repo = repo;

        public Task<VentaDetalleModel> Execute(int ventaId) => _repo.GetDetalleAsync(ventaId);
    }

    public class ActualizarClienteUseCase
    {
        private readonly IOptionVenta _repo;
        public ActualizarClienteUseCase(IOptionVenta repo) => _repo = repo;

        public Task<bool> Execute(int ventaId, VentaClienteRequest cliente)
            => _repo.ActualizarClienteAsync(ventaId, cliente);
    }

    public class CerrarVentaUseCase
    {
        private readonly IOptionVenta _repo;
        public CerrarVentaUseCase(IOptionVenta repo) => _repo = repo;

        public Task<bool> Execute(CerrarVentaRequest request) => _repo.CerrarAsync(request);
    }
}
