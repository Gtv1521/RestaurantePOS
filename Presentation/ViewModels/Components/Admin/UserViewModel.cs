using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MiComanderaApp.Models;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.ViewModels.Components.Admin
{
    public partial class UserViewModel : ViewModelBase
    {
        [ObservableProperty] private int _id = 0;
        [ObservableProperty] private string _nombreCompleto = "";
        [ObservableProperty] private string _iniciales = "";
        [ObservableProperty] private string _email = "";
        [ObservableProperty] private string _rol = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Estado))]
        private bool _activo = false;
        [ObservableProperty] private DateTime? _fechaCreacion;
        [ObservableProperty] private DateTime? _ultimoAcceso;

        public void Initialize(UsuarioModel usuario)
        {
            Id = usuario.Id;
            NombreCompleto = usuario.NombreCompleto;
            Email = usuario.Email;
            Iniciales = usuario.Iniciales;
            Rol = usuario.Rol;
            Activo = usuario.Activo;
            FechaCreacion = usuario.FechaCreacion;
            UltimoAcceso = usuario.UltimoAcceso;
        }

        public string Estado => Activo ? "Activo" : "Inactivo";
    }
}