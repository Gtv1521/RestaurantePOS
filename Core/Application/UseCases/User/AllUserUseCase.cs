using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Infrastructure.Api;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Request;

namespace MiComanderaApp.Core.Application.UseCases.User
{
    public class AllUserUseCase
    {
        private readonly IMultipleCrud<UsuarioModel, UserRequest> _repo;

        public AllUserUseCase(IMultipleCrud<UsuarioModel, UserRequest> repo)
        {
            _repo = repo;
        }

        public async Task<List<UsuarioModel>> ExecuteAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.ToList();
        }
    }
}