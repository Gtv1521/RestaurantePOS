using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Infrastructure.SignalR;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels.Routes;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MiComanderaApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly IViewModelFactory _factory;
    private readonly ISession<SessionModel> _sessionService;
    private readonly SignalRService _signalR;
    private readonly CultureInfo _culturaEspanol = new CultureInfo("es-ES");

    public LoginViewModel(INavigationService navigation, ISession<SessionModel> sessionService, IViewModelFactory factory, SignalRService signalR)
    {
        _navigation = navigation;
        _factory = factory;
        _signalR = signalR;
        _sessionService = sessionService;

        ActualizarHora();
        ConnectionServer();

        // Temporizador que corre cada segundo
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += (sender, e) => ActualizarHora();
        timer.Start();

    }

    [ObservableProperty] private string _pinCode = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string _horas = "00";
    [ObservableProperty] private string _minutos = "00";
    [ObservableProperty] private string _segundos = "00";
    [ObservableProperty] private string _fechaActual = string.Empty;
    [ObservableProperty] private string _connection = string.Empty;

    [RelayCommand]
    private void AddDigit(string digit)
    {
        if (PinCode.Length < 4)
        {
            PinCode += digit;
        }
    }

    [RelayCommand]
    private void DeleteDigit()
    {
        if (PinCode.Length > 0)
        {
            PinCode = PinCode.Substring(0, PinCode.Length - 1);
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            ErrorMessage = null;
            var session = await _sessionService.LoginAsync(PinCode);

            PinCode = string.Empty;
            if (session.Rol.Equals("Mesero", StringComparison.OrdinalIgnoreCase))
            {
                _navigation.NavigateTo<TablesViewModel>();
            }
            if (session.Rol.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
            {
                _navigation.NavigateTo<AdminDashboardViewModel>();
            }
        }
        catch (System.Exception ex)
        {
            ErrorMessage = ex.Message;
            PinCode = string.Empty;
        }
    }

    private void ActualizarHora()
    {
        // Formato de 24 horas (HH:mm:ss). Si prefieres 12 horas con AM/PM usa "hh:mm:ss tt"
        var ahora = DateTime.Now;

        // Extraemos cada componente con formato de dos dígitos ("00")
        Horas = ahora.ToString("HH");
        Minutos = ahora.ToString("mm");
        Segundos = ahora.ToString("ss");

        string fechaFormateada = ahora.ToString("dddd, d MMM yyyy", _culturaEspanol);
        FechaActual = fechaFormateada.ToUpper();
    }


    private void ConnectionServer()
    {
        _signalR.Connected += () => Connection = "🟢 Servidor conectado de forma segura";
        _signalR.Reconnecting += () => Connection = "🔵 Reconectando a servidor";
        _signalR.Disconnected += () => Connection = "🔴 Servidor desconectado";
    }

}