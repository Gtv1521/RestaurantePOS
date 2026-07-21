using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Models;
using RestaurantePOS.Core.Application.UseCases.Ingrediente;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.ViewModels;
using System.Diagnostics;

namespace MiComanderaApp.ViewModels.Components.Admin
{
    public partial class InventarioComponentViewModel : ViewModelBase
    {
        private readonly GetAllIngredientesUseCase _getAllIngredientesUseCase;

        [ObservableProperty]
        private ObservableCollection<IngredienteModel> _ingredientes = new();

        [ObservableProperty]
        private IngredienteModel? _selectedIngrediente;


        public InventarioComponentViewModel(GetAllIngredientesUseCase getAllIngredientesUseCase)
        {
            _getAllIngredientesUseCase = getAllIngredientesUseCase;
            _ = LoadIngredientesAsync();
        }

        private async Task LoadIngredientesAsync()
        {
            var ingredientesList = await _getAllIngredientesUseCase.Execute();
            foreach (var item in ingredientesList)
            {
                Ingredientes.Add(item);
                System.Console.WriteLine(item.Name);
            }
        }

        [RelayCommand]
        private void AddIngrediente()
        {
            // TODO: Implementar la lógica para abrir el modal de agregar ingrediente
            Debug.WriteLine("Add Ingrediente action");
        }

        [RelayCommand]
        private void EditIngrediente(IngredienteModel ingrediente)
        {
            if (ingrediente == null) return;
            // TODO: Implementar la lógica para abrir el modal de edición
            Debug.WriteLine($"Edit Ingrediente: {ingrediente.Name}");
        }

        [RelayCommand]
        private void DeleteIngrediente(IngredienteModel ingrediente)
        {
            if (ingrediente == null) return;
            // TODO: Implementar la lógica para confirmar y eliminar el ingrediente
            Debug.WriteLine($"Delete Ingrediente: {ingrediente.Name}");
        }
    }
}