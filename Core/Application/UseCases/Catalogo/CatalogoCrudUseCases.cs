using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Catalogo
{
    public class CreateCatalogoUseCase
    {
        private readonly IMultipleCrud<CatalogoModel, CatalogoRequest> _repo;
        public CreateCatalogoUseCase(IMultipleCrud<CatalogoModel, CatalogoRequest> repo)
        {
            _repo = repo;
        }

        public Task<int?> Execute(CatalogoRequest request) => _repo.CreateAsync(request);
    }

    public class DeleteCatalogoUseCase
    {
        private readonly IMultipleCrud<CatalogoModel, CatalogoRequest> _repo;
        public DeleteCatalogoUseCase(IMultipleCrud<CatalogoModel, CatalogoRequest> repo)
        {
            _repo = repo;
        }

        public Task<bool> Execute(int id) => _repo.DeleteAsync(id);
    }
}
