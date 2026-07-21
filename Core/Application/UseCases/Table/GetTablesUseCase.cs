using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Table
{
    public class GetTablesUseCase
    {
        private readonly ISingleCrud<TableModel, TableRequest> _repo;

        public GetTablesUseCase(ISingleCrud<TableModel, TableRequest> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TableModel>> Execute(string id, int? page, int? size)
        {
            return await _repo.GetAllAsync(id, page, size);
        }
    }
}