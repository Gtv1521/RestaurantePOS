using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Table
{
    public class GetTablesOpenUseCase
    {
        private readonly IGetOpens<VentaModel> _repo;

        public GetTablesOpenUseCase(IGetOpens<VentaModel> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<VentaModel>> Execute()
        {
            return await _repo.TablesOpen();
        }
    }
}