using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using RestaurantePOS.Core.Application.Request;

namespace RestaurantePOS.Core.Application.UseCases.Ingrediente
{
    public class DeleteIngredienteUseCase
    {
        private readonly IMultipleCrud<IngredienteModel, IngredienteRequest> _repo;

        public DeleteIngredienteUseCase(IMultipleCrud<IngredienteModel, IngredienteRequest> repo)
        {
            _repo = repo;
        }
        public async Task<bool> Execute(string id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}