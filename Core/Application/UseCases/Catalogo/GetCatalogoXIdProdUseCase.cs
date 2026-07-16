using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Catalogo
{
    public class GetCatalogoXIdProdUseCase
    {
        private readonly IGetList<ProductoModel> _repo;

        public GetCatalogoXIdProdUseCase(IGetList<ProductoModel> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ProductoModel>> Execute(int id)
        {
            return await _repo.GetAllDataAsync(id);
        }

    }
}