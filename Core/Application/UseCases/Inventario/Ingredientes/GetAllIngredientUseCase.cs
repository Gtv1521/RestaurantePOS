using System.Collections.Generic;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Inventario.Ingredientes
{
    public class GetAllIngredientesUseCase
    {
        private readonly IMultipleCrud<IngredienteModel, IngredienteRequest> _repo;

        public GetAllIngredientesUseCase(IMultipleCrud<IngredienteModel, IngredienteRequest> repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<IngredienteModel>> ExecuteAsync()
        {
            return await _repo.GetAllAsync();
        }

    }
}