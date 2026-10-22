using System;
using System.Threading;
using System.Threading.Tasks;
using MiComanderaApp.Core.Application.Request;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Core.Infrastructure.Api;

namespace MiComanderaApp.Core.Application.Services
{
    /// <summary>
    /// Cache compartido de la configuración general. Se carga una vez y se
    /// refresca tras guardar, para que la venta y la impresión usen los
    /// valores configurados sin depender de valores hardcodeados.
    /// </summary>
    public sealed class GeneralSettingsStore
    {
        private readonly GeneralSettingsRepository _repo;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public GeneralSettingsStore(GeneralSettingsRepository repo) => _repo = repo;

        public GeneralSettingsModel Current { get; private set; } = CreateDefaults();

        public event Action? Changed;

        public async Task<GeneralSettingsModel> RefreshAsync()
        {
            await _gate.WaitAsync();
            try
            {
                var settings = await _repo.GetAsync();
                if (settings != null)
                {
                    Current = settings;
                    Changed?.Invoke();
                }
            }
            catch
            {
                // Si el servidor no responde se mantienen los valores por defecto.
            }
            finally
            {
                _gate.Release();
            }

            return Current;
        }

        public async Task<GeneralSettingsModel?> SaveAsync(GeneralSettingsRequest request)
        {
            await _gate.WaitAsync();
            try
            {
                var settings = await _repo.UpdateAsync(request);
                if (settings != null)
                {
                    Current = settings;
                    Changed?.Invoke();
                }

                return settings;
            }
            finally
            {
                _gate.Release();
            }
        }

        private static GeneralSettingsModel CreateDefaults() => new()
        {
            BusinessName = "RESTAURANTE",
            CurrencySymbol = "$",
            CurrencyCode = "COP",
            DecimalPlaces = 2,
            TaxEnabled = true,
            TaxPercentage = 8m,
            TaxIncludedInPrice = true,
            TaxLabel = "Impoconsumo",
            ServiceChargeEnabled = true,
            ServiceChargePercentage = 10m,
            ServiceChargeLabel = "Servicio",
            TicketWidthChars = 42,
            TicketFooter = "¡GRACIAS POR SU VISITA!",
            DefaultPax = 2,
            DefaultPrinterPort = 9100,
        };
    }
}
