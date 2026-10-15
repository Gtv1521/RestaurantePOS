using System.Threading.Tasks;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Request;

namespace MiComanderaApp.Core.Application.UseCases.User
{
    public class CreateUserUseCase
    {
        private readonly IMultipleCrud<UsuarioModel, UserRequest> _repo;

        public CreateUserUseCase(IMultipleCrud<UsuarioModel, UserRequest> repo)
        {
            _repo = repo;
        }

        public Task<int?> ExecuteAsync(UserRequest data) => _repo.CreateAsync(data);
    }
}
