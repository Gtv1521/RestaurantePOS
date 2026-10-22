using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.UseCases.Configuracion
{
    public class GetGeneralSettingsUseCase
    {
        private readonly GeneralSettingsRepository _repo;
        public GetGeneralSettingsUseCase(GeneralSettingsRepository repo) => _repo = repo;
        public Task<GeneralSettingsModel?> Execute() => _repo.GetAsync();
    }

    public class UpdateGeneralSettingsUseCase
    {
        private readonly GeneralSettingsRepository _repo;
        public UpdateGeneralSettingsUseCase(GeneralSettingsRepository repo) => _repo = repo;
        public Task<GeneralSettingsModel?> Execute(GeneralSettingsRequest request) => _repo.UpdateAsync(request);
    }
}
