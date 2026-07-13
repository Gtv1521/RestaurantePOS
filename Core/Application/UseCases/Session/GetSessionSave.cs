using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;

namespace MiComanderaApp.Core.Application.UseCases.Session
{
    public class GetSessionSave
    {
        private readonly ISession<SessionModel> _repo;

        public GetSessionSave(ISession<SessionModel> repo)
        {
            _repo = repo;
        }

        public SessionModel Execute()
        {
            return _repo.Model!;
        }
    }
}