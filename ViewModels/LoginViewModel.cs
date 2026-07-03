using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using System;
using System.Threading.Tasks;

namespace MiComanderaApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    public LoginViewModel(INavigationService navigation) => _navigation = navigation;

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
        ErrorMessage = null;
        // TODO: Reemplaza "1234" con tu lógica de validación de PIN real.
        if (PinCode == "1234")
        {
            // Simula una pequeña espera como si estuviera verificando
            await Task.Delay(250);
            _navigation.NavigateTo<TablesViewModel>();
        }
        else
        {
            ErrorMessage = "PIN incorrecto. Inténtalo de nuevo.";
            PinCode = string.Empty;
        }
    }
}