using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Table
{
    public class GetAllTablesUseCase
    {
        private readonly IMultipleCrud<TableModel, TableRequest> _repo;

        public GetAllTablesUseCase(IMultipleCrud<TableModel, TableRequest> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<TableModel>> Execute()
        {
            var result = await _repo.GetAllAsync();
            return result;
        }
    }
}