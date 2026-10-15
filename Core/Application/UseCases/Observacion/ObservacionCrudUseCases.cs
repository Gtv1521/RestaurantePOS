using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Observacion
{
    public class CreateObservacionUseCase
    {
        private readonly IMultipleCrud<ObservacionModel, ObservacionRequest> _repo;
        public CreateObservacionUseCase(IMultipleCrud<ObservacionModel, ObservacionRequest> repo) => _repo = repo;
        public Task<int?> Execute(ObservacionRequest request) => _repo.CreateAsync(request);
    }

    public class DeleteObservacionUseCase
    {
        private readonly IMultipleCrud<ObservacionModel, ObservacionRequest> _repo;
        public DeleteObservacionUseCase(IMultipleCrud<ObservacionModel, ObservacionRequest> repo) => _repo = repo;
        public Task<bool> Execute(int id) => _repo.DeleteAsync(id);
    }
}
