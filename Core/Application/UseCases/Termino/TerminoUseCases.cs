using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Termino
{
    public class GetAllTerminosUseCase
    {
        private readonly TerminoRepository _repo;
        public GetAllTerminosUseCase(TerminoRepository repo) => _repo = repo;
        public Task<IEnumerable<TerminoModel>> Execute() => _repo.GetAllAsync();
    }

    public class CreateTerminoUseCase
    {
        private readonly TerminoRepository _repo;
        public CreateTerminoUseCase(TerminoRepository repo) => _repo = repo;
        public Task<TerminoModel?> Execute(TerminoRequest request) => _repo.CreateAsync(request);
    }

    public class DeleteTerminoUseCase
    {
        private readonly TerminoRepository _repo;
        public DeleteTerminoUseCase(TerminoRepository repo) => _repo = repo;
        public Task Execute(int terminoId) => _repo.DeleteAsync(terminoId);
    }
}
