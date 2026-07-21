using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Infrastructure.Api
{
    public class TablesRepository : ISingleCrud<TableModel, TableRequest>
    {
        public Task<string?> CreateAsync(TableRequest data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TableModel>> GetAllAsync(string id, int? page, int? size)
        {
            throw new NotImplementedException();
        }

        public Task<TableModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(string id, TableRequest data)
        {
            throw new NotImplementedException();
        }
    }
}