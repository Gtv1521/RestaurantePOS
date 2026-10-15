using CommunityToolkit.Mvvm.ComponentModel;

namespace MiComanderaApp.Presentation.Services
{
    /// <summary>
    /// Estado de carga global de la app. Singleton: cualquier pantalla puede
    /// mostrar el overlay con Loading.Show("texto") y ocultarlo con Hide().
    /// </summary>
    public partial class LoadingState : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _statusText = "Cargando...";

        public void Show(string? statusText = null)
        {
            if (!string.IsNullOrWhiteSpace(statusText))
                StatusText = statusText;
            IsLoading = true;
        }

        public void Hide() => IsLoading = false;
    }
}
