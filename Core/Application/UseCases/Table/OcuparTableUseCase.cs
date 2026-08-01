using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Table
{
    public class OcuparTableUseCase
    {
        private readonly IOptionsMesas<VentaModel> _repo;
        public OcuparTableUseCase(IOptionsMesas<VentaModel> repo)
        {
            _repo = repo;
        }

        public async Task<VentaModel> Execute(int id)
        {
            var result = await _repo.OcuparMesa(id);
            return result;
        }
    }
}