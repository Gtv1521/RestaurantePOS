using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Interfaces;
using RestaurantePOS.Core.Domain.Models;

namespace RestaurantePOS.Core.Application.UseCases.Receta
{
    public class GetAllRecetasUseCase
    {
        private readonly IRecetaRepository _recetaRepository;
        public GetAllRecetasUseCase(IRecetaRepository recetaRepository )
        {
            _recetaRepository = recetaRepository;
        }

        public async Task<List<RecetaModel>> ExecuteAsync(int pageNumber, int pageSize)
        {
            return await _recetaRepository.GetAllRecetasAsync(pageNumber, pageSize);
        }
        
    }
}