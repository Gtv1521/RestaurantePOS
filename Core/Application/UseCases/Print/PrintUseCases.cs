using System.Collections.Generic;
using System.Threading.Tasks;
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

    public class ImprimirDocumentoUseCase
    {
        private readonly ImpresoraRepository _repo;
        public ImprimirDocumentoUseCase(ImpresoraRepository repo) => _repo = repo;

        public Task<bool> Execute(int impresoraId, string texto)
            => _repo.ImprimirAsync(impresoraId, texto);
    }
}
