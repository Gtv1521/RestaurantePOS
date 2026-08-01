using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Observacion
{
    public class GetAllObservacionUseCase
    {
        private readonly IMultipleCrud<ObservacionModel, ObservacionRequest> _repo;

        public GetAllObservacionUseCase(IMultipleCrud<ObservacionModel, ObservacionRequest> repo)
        {
            _repo = repo;
        }

        public async Task<List<ObservacionModel>> Execute()
        {
            var result = await _repo.GetAllAsync();
            return result.ToList();
        }
    }
}