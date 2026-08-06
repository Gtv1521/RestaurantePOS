using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using RestaurantePOS.Core.Domain.Models;
using RestaurantePOS.Core.Application.UseCases.Receta;
using System.Collections.Generic;
using System.Linq;
using System;
using MiComanderaApp.ViewModels.Dialogs.Modals;
using MiComanderaApp.Core.Domain.Interfaces;
using MiComanderaApp.Views.Dialogs.Modals;
using Avalonia;
using RestaurantePOS.Core.Application.UseCases.Ingrediente;
using MiComanderaApp.Core.Application.Request;

namespace MiComanderaApp.ViewModels.Components.Admin
{
    public partial class InventarioComponentViewModel : ViewModelBase
    {
        private readonly GetAllIngredientesUseCase _getAllIngredientesUseCase;
        private readonly GetAllRecetasUseCase _getAllRecetasUseCase;
        private readonly CreateIngredienteUseCase _createIngredienteUseCase;
        private readonly EditIngredienteUseCase _editIngredienteUseCase;

        [ObservableProperty]

        private List<IngredienteModel> _allIngredientes = new();
        private List<RecetaModel> _allRecetas = new();

        [ObservableProperty]
        private ObservableCollection<IngredienteModel> _ingredientes = new();

        [ObservableProperty]
        private ObservableCollection<RecetaModel> _recetas = new();

        [ObservableProperty]
        private IngredienteModel? _selectedIngrediente;

        [ObservableProperty]
        private bool _isIngredientesSelected = true;

        [ObservableProperty]
        private string _searchText = string.Empty;

        private readonly IDialogService _dialogService;

        public InventarioComponentViewModel(
            GetAllIngredientesUseCase getAllIngredientesUseCase, 
            GetAllRecetasUseCase getAllRecetasUseCase, 
            IDialogService dialogService, 
            CreateIngredienteUseCase createIngredienteUseCase, 
            EditIngredienteUseCase editIngredienteUseCase
        )
        {
            _getAllIngredientesUseCase = getAllIngredientesUseCase;
            _getAllRecetasUseCase = getAllRecetasUseCase;
            _dialogService = dialogService;
            _createIngredienteUseCase = createIngredienteUseCase;
            _editIngredienteUseCase = editIngredienteUseCase;
            LoadIngredientesCommand.Execute(null);
        }

        partial void OnSearchTextChanged(string value)
        {
            if (IsIngredientesSelected)
            {
                FilterIngredientes();
            }
            else
            {
                FilterRecetas();
            }
        }

        private void FilterIngredientes()
        {
            Ingredientes.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? AllIngredientes : AllIngredientes.Where(i => i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                Ingredientes.Add(item);
            }
        }

        private void FilterRecetas()
        {
            Recetas.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allRecetas
                : _allRecetas.Where(r => r.ProductName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                Recetas.Add(item);
            }
        }

        private async Task LoadIngredientesAsync()
        {
            var ingredientesList = await _getAllIngredientesUseCase.Execute();
            AllIngredientes = new List<IngredienteModel>(ingredientesList);
            FilterIngredientes();
        }

        private async Task LoadRecetasAsync()
        {
            var recetasList = await _getAllRecetasUseCase.ExecuteAsync(1, 100);
            _allRecetas = new List<RecetaModel>(recetasList);
            FilterRecetas();
        }

        [RelayCommand]
        private async Task LoadIngredientes()
        {
            IsIngredientesSelected = true;
            await LoadIngredientesAsync();
        }

        [RelayCommand]
        private async Task LoadRecetas()
        {
            IsIngredientesSelected = false;
            await LoadRecetasAsync();
        }

        [RelayCommand]
        private async Task AddIngrediente()
        {
            dynamic dialogService = _dialogService;
            var result = await dialogService.ShowDialogAsync<CreateIngrediente, CreateIngredienteDialogViewModel, IngredienteModel>(new PixelPoint(250, 30));
            if (result is IngredienteModel newIngredient)
            {
                var request = new IngredienteRequest
                {
                    Name = newIngredient.Name,
                    InitialQuantity = newIngredient.AvailableQuantity,
                    MinimumQuantity = newIngredient.MinimumQuantity,
                    UnitCost = newIngredient.UnitCost,
                    UnitOfMeasure = newIngredient.UnitOfMeasure
                };
                var response = await _createIngredienteUseCase.Execute(request);
                if (response != null)
                {
                    await LoadIngredientesAsync();
                    FilterIngredientes();
                }
            }
        }
        
        [RelayCommand]
        private void NewRecipe()
        {
            Debug.WriteLine("New Recipe action");
        }

        [RelayCommand]
        private async Task EditIngrediente(IngredienteModel ingrediente)
        {
            if (ingrediente == null) return;

            // 1. Creamos el ViewModel para el diálogo, pasándole el ingrediente a editar.
            var viewModel = new EditIngredienteDialogViewModel(ingrediente);

            // 2. Mostramos el diálogo y le pasamos el ViewModel ya inicializado.
            var result = await _dialogService.ShowDialogAsync<EditIngrediente, EditIngredienteDialogViewModel, IngredienteRequest?>(viewModel, new PixelPoint(250, 30));

            // 3. Si el usuario guardó (result no es null), procesamos la actualización.
            if (result is IngredienteRequest updatedIngrediente)
            {
                var response = await _editIngredienteUseCase.Execute(updatedIngrediente);
                if (response != null)
                    await LoadIngredientesAsync();
            }
        }

        [RelayCommand]
        private void DeleteIngrediente(IngredienteModel ingrediente)
        {
            if (ingrediente == null) return;
            // TODO: Implementar la lógica para confirmar y eliminar el ingrediente
            Debug.WriteLine($"Delete Ingrediente: {ingrediente.Name}");
        }

        [RelayCommand]
        private void EditRecipe(RecetaModel receta)
        {
            if (receta == null) return;
            // TODO: Implementar la lógica para abrir el modal de edición de receta
            Debug.WriteLine($"Edit Recipe: {receta.ProductName}");
        }

        [RelayCommand]
        private void DeleteRecipe(RecetaModel receta)
        {
            if (receta == null) return;
            // TODO: Implementar la lógica para confirmar y eliminar la receta
            Debug.WriteLine($"Delete Recipe: {receta.ProductName}");
        }
    }
}
