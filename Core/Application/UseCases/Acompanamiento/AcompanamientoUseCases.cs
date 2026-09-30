using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Acompanamiento
{
    public class GetAllAcompanamientosUseCase
    {
        private readonly AcompanamientoRepository _repo;
        public GetAllAcompanamientosUseCase(AcompanamientoRepository repo) => _repo = repo;
        public Task<IEnumerable<AcompanamientoModel>> Execute() => _repo.GetAllAsync();
    }

    public class CreateAcompanamientoUseCase
    {
        private readonly AcompanamientoRepository _repo;
        public CreateAcompanamientoUseCase(AcompanamientoRepository repo) => _repo = repo;
        public Task<AcompanamientoModel?> Execute(AcompanamientoRequest request) => _repo.CreateAsync(request);
    }

    public class DeleteAcompanamientoUseCase
    {
        private readonly AcompanamientoRepository _repo;
        public DeleteAcompanamientoUseCase(AcompanamientoRepository repo) => _repo = repo;
        public Task Execute(int acompanamientoId) => _repo.DeleteAsync(acompanamientoId);
    }
}
