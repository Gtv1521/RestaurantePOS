using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace RestaurantePOS.Core.Application.UseCases.Ingrediente
{
    public class EditIngredienteUseCase
    {
        private readonly IMultipleCrud<IngredienteModel, IngredienteRequest> _repo;
        public EditIngredienteUseCase(IMultipleCrud<IngredienteModel, IngredienteRequest> repo)
        {
            _repo = repo;
        }

        public async Task<string?> Execute(IngredienteRequest data)
        {
            var response = await _repo.UpdateAsync(data.Id.ToString(), data);
            return response.CompareTo(true) == 0 ? "Ingrediente actualizado correctamente" : "Error al actualizar el ingrediente";
        }
    }
}