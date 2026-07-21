using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Product
{
    public class GetAllProductUseCase
    {
        private readonly IMultipleCrud<ProductoModel, ProductoRequest> _repo;

        public GetAllProductUseCase(IMultipleCrud<ProductoModel, ProductoRequest> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProductoModel>> Execute()
        {
            return await _repo.GetAllAsync();
        }
    }
}