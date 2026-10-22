using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Print
{
    public class GetImpresorasUseCase
    {
        private readonly ImpresoraRepository _repo;
        public GetImpresorasUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<List<ImpresoraModel>> Execute() => _repo.GetImpresorasAsync();
    }

    public class CreateImpresoraUseCase
    {
        private readonly ImpresoraRepository _repo;
        public CreateImpresoraUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<ImpresoraModel?> Execute(ImpresoraRequest request) => _repo.CreateAsync(request);
    }

    public class UpdateImpresoraUseCase
    {
        private readonly ImpresoraRepository _repo;
        public UpdateImpresoraUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<ImpresoraModel?> Execute(int id, ImpresoraRequest request) => _repo.UpdateAsync(id, request);
    }

    public class DeleteImpresoraUseCase
    {
        private readonly ImpresoraRepository _repo;
        public DeleteImpresoraUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task Execute(int id) => _repo.DeleteAsync(id);
    }

    public class DiscoverImpresorasUseCase
    {
        private readonly ImpresoraRepository _repo;
        public DiscoverImpresorasUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<List<ImpresoraDescubiertaModel>> Execute(string? subnet) => _repo.DiscoverAsync(subnet);
    }

    public class RegisterDiscoveredImpresoraUseCase
    {
        private readonly ImpresoraRepository _repo;
        public RegisterDiscoveredImpresoraUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<ImpresoraModel?> Execute(ImpresoraRequest request) => _repo.RegisterDiscoveredAsync(request);
    }

    public class ImprimirDocumentoUseCase
    {
        private readonly ImpresoraRepository _repo;
        public ImprimirDocumentoUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<bool> Execute(int impresoraId, string texto)
            => _repo.ImprimirAsync(impresoraId, texto);
    }
}
