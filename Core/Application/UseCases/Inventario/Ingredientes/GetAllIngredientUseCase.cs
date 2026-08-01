using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Inventario.Ingredientes
{
    public class GetAllIngredientesUseCase
    {
        private readonly IngredienteRepository _repository;

        public GetAllIngredientesUseCase(IngredienteRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<IngredienteModel>> ExecuteAsync()
        {
            return await _repository.GetAllAsync();
        }

    }
}