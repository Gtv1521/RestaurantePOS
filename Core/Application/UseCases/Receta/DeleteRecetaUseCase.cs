using MiComanderaApp.Core.Domain.Interfaces;
using System.Threading.Tasks;

namespace RestaurantePOS.Core.Application.UseCases.Receta
{
    public class DeleteRecetaUseCase
    {
        private readonly IRecetaRepository _recetaRepository;

        public DeleteRecetaUseCase(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<bool> Execute(string id)
        {
            return await _recetaRepository.DeleteAsync(id);
        }
    }
}
