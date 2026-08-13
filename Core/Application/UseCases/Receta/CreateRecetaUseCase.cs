using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using System.Threading.Tasks;

namespace RestaurantePOS.Core.Application.UseCases.Receta
{
    public class CreateRecetaUseCase
    {
        private readonly IRecetaRepository _recetaRepository;

        public CreateRecetaUseCase(IRecetaRepository recetaRepository)
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<string?> Execute(RecetaRequest request)
        {
            return await _recetaRepository.CreateAsync(request);
        }
    }
}
