using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using System;
using System.Threading.Tasks;

namespace MiComanderaApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly ISession<SessionModel> _sessionService;
    public LoginViewModel(INavigationService navigation, ISession<SessionModel> sessionService)
    {
        _navigation = navigation;
        _sessionService = sessionService;
    }

    [ObservableProperty]
    private string _pinCode = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;


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
            if (session?.UserId != null)
            {
                _navigation.NavigateTo<TablesViewModel>();
                PinCode = string.Empty;
            }

        }
        catch (System.Exception ex)
        {
            ErrorMessage = ex.Message;
            PinCode = string.Empty;
        }
    }
}