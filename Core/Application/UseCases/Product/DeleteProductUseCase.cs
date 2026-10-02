using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Product
{
    public class DeleteProductUseCase
    {
        private readonly IMultipleCrud<ProductoModel, ProductoRequest> _repo;

        public DeleteProductUseCase(IMultipleCrud<ProductoModel, ProductoRequest> repo)
        {
            _repo = repo;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

    }
}