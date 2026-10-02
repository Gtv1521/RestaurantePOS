using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using System.Threading.Tasks;

namespace RestaurantePOS.Core.Application.UseCases.Receta
{
    public class UpdateRecetaUseCase
    {
        private readonly IRecetaRepository _recetaRepository;

        public UpdateRecetaUseCase(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<bool> Execute(int id, RecetaRequest request)
        {
            return await _recetaRepository.UpdateAsync(id, request);
        }
    }
}
