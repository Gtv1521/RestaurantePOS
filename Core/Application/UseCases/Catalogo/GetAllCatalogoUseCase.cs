using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Catalogo
{
    public class GetAllCatalogoUseCase
    {
        private readonly IMultipleCrud<CatalogoModel, CatalogoRequest> _repo;
        public GetAllCatalogoUseCase(IMultipleCrud<CatalogoModel, CatalogoRequest> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CatalogoModel>> Execute()
        {
            return await _repo.GetAllAsync();
        }
    }
}