using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Product
{
    public class InsertProductUseCase
    {
        private readonly IMultipleCrud<ProductoModel, ProductoRequest> _repo;

        public InsertProductUseCase(IMultipleCrud<ProductoModel, ProductoRequest> repo)
        {
            _repo = repo;
        }

        public async Task<string> Execute(ProductoRequest request)
        {
            return await _repo.CreateAsync(request);
        }
    }
}