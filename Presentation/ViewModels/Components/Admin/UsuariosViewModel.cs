using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.Views.Components.Admin;

public partial class UsuariosViewModel : ViewModelBase
{
    // private readonly IViewModelFactory _factory;
    // private readonly IUserService _userService;
    // private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<UsuarioModel> _users = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedFilter = "Todos";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _totalUsuarios;

    [ObservableProperty]
    private int _usuariosActivos;

    [ObservableProperty]
    private int _usuariosInactivos;

    public ObservableCollection<string> FilterOptions { get; } = new()
        {
            "Todos",
            "Activos",
            "Inactivos",
            "Administradores",
            "Cajeros",
            "Meseros",
            "Inventario"
        };

    public UsuariosViewModel(
        IViewModelFactory factory)
    {
        // _factory = factory;

        LoadUsers();
    }

    private async void LoadUsers()
    {
        await CargarUsuariosAsync();
    }

    [RelayCommand]
    public async Task CargarUsuariosAsync()
    {
        // try
        // {
        //     IsLoading = true;
        //     // var users = await _userService.GetAllUsersAsync();

        //     // Users.Clear();
        //     // foreach (var user in users)
        //     // {
        //     //     Users.Add(user);
        //     // }

        //     ActualizarEstadisticas();
        // }
        // catch (Exception ex)
        // {
        //     // await _dialogService.ShowErrorAsync("Error", $"No se pudieron cargar los usuarios: {ex.Message}");
        // }
        // finally
        // {
        //     IsLoading = false;
        // }
    }

    [RelayCommand]
    public async Task AgregarUsuarioAsync()
    {
        // var dialogVm = _factory.Create<UserDialogViewModel>();
        // dialogVm.InitializeForCreate();

        // var result = await _dialogService.ShowDialogAsync<UserModel>(dialogVm);
        // if (result != null)
        // {
        //     var newUser = new UserItemViewModel(result);
        //     Users.Add(newUser);
        //     ActualizarEstadisticas();
        //     await _dialogService.ShowSuccessAsync("Éxito", "Usuario agregado correctamente");
        // }
    }

    [RelayCommand]
    public async Task EditarUsuarioAsync(UsuarioModel userItem)
    {
        // if (userItem == null) return;

        // var dialogVm = _factory.Create<UsuarioDialogViewModel>();
        // dialogVm.InitializeForEdit(userItem.User);

        // var result = await _dialogService.ShowDialogAsync<UserModel>(dialogVm);
        // if (result != null)
        // {
        //     // Actualizar el usuario en la lista
        //     var index = Users.IndexOf(userItem);
        //     Users[index] = new UserItemViewModel(result);
        //     ActualizarEstadisticas();
        //     await _dialogService.ShowSuccessAsync("Éxito", "Usuario actualizado correctamente");
        // }
    }

    [RelayCommand]
    public async Task EliminarUsuarioAsync(UsuarioModel userItem)
    {
        // if (userItem == null) return;

        // var confirm = await _dialogService.ShowConfirmAsync(
        //     "Confirmar eliminación",
        //     $"¿Estás seguro de eliminar al usuario {userItem.NombreCompleto}?");

        // if (confirm)
        // {
        //     try
        //     {
        //         await _userService.DeleteUserAsync(userItem.Id);
        //         Users.Remove(userItem);
        //         ActualizarEstadisticas();
        //         await _dialogService.ShowSuccessAsync("Éxito", "Usuario eliminado correctamente");
        //     }
        //     catch (Exception ex)
        //     {
        //         await _dialogService.ShowErrorAsync("Error", $"No se pudo eliminar el usuario: {ex.Message}");
        //     }
        // }
    }

    [RelayCommand]
    public async Task ToggleEstadoAsync(UsuarioModel userItem)
    {
        // if (userItem == null) return;

        // var nuevoEstado = !userItem.Activo;
        // var estadoTexto = nuevoEstado ? "activar" : "desactivar";

        // var confirm = await _dialogService.ShowConfirmAsync(
        //     "Confirmar",
        //     $"¿Deseas {estadoTexto} al usuario {userItem.NombreCompleto}?");

        // if (confirm)
        // {
        //     try
        //     {
        //         userItem.Activo = nuevoEstado;
        //         await _userService.UpdateUserAsync(userItem);
        //         ActualizarEstadisticas();
        //         await _dialogService.ShowSuccessAsync("Éxito", $"Usuario {estadoTexto} correctamente");
        //     }
        //     catch (Exception ex)
        //     {
        //         await _dialogService.ShowErrorAsync("Error", $"No se pudo {estadoTexto} el usuario: {ex.Message}");
        //     }
        // }
    }

    [RelayCommand]
    private void FiltrarUsuarios()
    {
        // Lógica de filtrado
        ActualizarEstadisticas();
    }

    private void ActualizarEstadisticas()
    {
        var filtered = string.IsNullOrEmpty(SearchText)
            ? Users
            : new ObservableCollection<UsuarioModel>(
                Users.Where(u => u.NombreCompleto.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

        TotalUsuarios = filtered.Count;
        UsuariosActivos = filtered.Count(u => u.Activo);
        UsuariosInactivos = filtered.Count(u => !u.Activo);
    }

    partial void OnSearchTextChanged(string value)
    {
        ActualizarEstadisticas();
    }
}
