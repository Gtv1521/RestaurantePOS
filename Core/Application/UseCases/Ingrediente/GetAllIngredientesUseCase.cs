using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using RestaurantePOS.Core.Application.Request;

namespace RestaurantePOS.Core.Application.UseCases.Ingrediente
{
    public class GetAllIngredientesUseCase
    {
        private readonly IMultipleCrud<IngredienteModel, IngredienteRequest> _repo;
        public GetAllIngredientesUseCase(IMultipleCrud<IngredienteModel, IngredienteRequest> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<IngredienteModel>> Execute()
        {
            return await _repo.GetAllAsync();
        }
    }
}