using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MiComanderaApp.Interfaces;

namespace MiComanderaApp.Core.Application.UseCases.Venta
{
    public class UpdatePaxUseCase
    {
        private readonly IOptionVenta _repo;

        public UpdatePaxUseCase(IOptionVenta repo)
        {
            _repo = repo;
        }

        public async Task<bool> ExecuteAsync(int id, int pax)
        {
            return await _repo.UptatePax(id, pax);
        }
    }
}