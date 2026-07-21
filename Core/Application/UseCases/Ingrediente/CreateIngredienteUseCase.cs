using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using RestaurantePOS.Core.Application.Request;

namespace RestaurantePOS.Core.Application.UseCases.Ingrediente
{
    public class CreateIngredienteUseCase
    {
        private readonly IMultipleCrud<IngredienteModel, IngredienteRequest> _repo;

        public CreateIngredienteUseCase(IMultipleCrud<IngredienteModel, IngredienteRequest> repo)
        {
            _repo = repo;
        }

        public async Task<string?> Execute(IngredienteRequest data)
        {
            return await _repo.CreateAsync(data);
        }
    
    }
}