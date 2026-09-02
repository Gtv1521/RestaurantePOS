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
using MiComanderaApp.Models;
using MiComanderaApp.Core.Application.UseCases.Product;
using MiComanderaApp.Core.Application.Request;
using System.Text.Json;

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

        private readonly GetAllProductUseCase _getAllProductUseCase;
        private readonly CreateRecetaUseCase _createRecetaUseCase;
        private readonly UpdateRecetaUseCase _updateRecetaUseCase;
        private readonly DeleteRecetaUseCase _deleteRecetaUseCase;

        public InventarioComponentViewModel(
            GetAllIngredientesUseCase getAllIngredientesUseCase,
            GetAllRecetasUseCase getAllRecetasUseCase,
            IDialogService dialogService,
            CreateIngredienteUseCase createIngredienteUseCase,
            EditIngredienteUseCase editIngredienteUseCase,
            GetAllProductUseCase getAllProductUseCase,
            CreateRecetaUseCase createRecetaUseCase,
            UpdateRecetaUseCase updateRecetaUseCase,
            DeleteRecetaUseCase deleteRecetaUseCase
        )
        {
            _getAllIngredientesUseCase = getAllIngredientesUseCase;
            _getAllRecetasUseCase = getAllRecetasUseCase;
            _dialogService = dialogService;
            _createIngredienteUseCase = createIngredienteUseCase;
            _editIngredienteUseCase = editIngredienteUseCase;
            _getAllProductUseCase = getAllProductUseCase;
            _createRecetaUseCase = createRecetaUseCase;
            _updateRecetaUseCase = updateRecetaUseCase;
            _deleteRecetaUseCase = deleteRecetaUseCase;
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
            var allProducts = await _getAllProductUseCase.Execute();
            var allRecipes = await _getAllRecetasUseCase.ExecuteAsync(1, 1000); // Assuming 1000 is enough

            var productRecipes = allProducts.Select(p =>
            {
                var existingRecipe = allRecipes.FirstOrDefault(r => r.ProductId == p.Id);
                if (existingRecipe != null)
                {
                    existingRecipe.HasRecipe = true;
                    return existingRecipe;
                }
                else
                {
                    return new RecetaModel
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        HasRecipe = false
                    };
                }
            }).ToList();
            
            _allRecetas = new List<RecetaModel>(productRecipes);
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
        private async Task NewRecipe()
        {
            Debug.WriteLine("NewRecipe command invoked");
            var allProducts = await _getAllProductUseCase.Execute();
            var allRecipes = await _getAllRecetasUseCase.ExecuteAsync(1, 1000); // Assuming 1000 is enough to get all recipes

            // Previously we only allowed creating recipes for products without recipe.
            // Open selector with all products so user can pick any product to create a recipe for.
            var productsWithoutRecipe = allProducts.Where(p => !allRecipes.Any(r => r.ProductId == p.Id)).ToList();
            var selectList = allProducts.ToList();

            var selectProductViewModel = new SelectProductForRecipeViewModel(selectList);
            Debug.WriteLine($"Opening SelectProduct dialog with {productsWithoutRecipe.Count} candidates");
            var selectedProduct = await _dialogService.ShowDialogAsync<SelectProductForRecipeViewModel, ProductoModel?>(selectProductViewModel);
            Debug.WriteLine($"SelectProduct dialog closed. Selected: {selectedProduct?.Name ?? "<null>"}");

            if (selectedProduct != null)
            {
                var newReceta = new RecetaModel
                {
                    ProductId = selectedProduct.Id,
                    ProductName = selectedProduct.Name,
                    Items = new List<RestaurantePOS.Core.Domain.Models.IngredientModel>()
                };

                var recipeEditorViewModel = new RecipeEditorViewModel(newReceta, _getAllIngredientesUseCase);
                var recipeResult = await _dialogService.ShowDialogAsync<RecipeEditorViewModel, RecetaModel?>(recipeEditorViewModel);

                if (recipeResult != null)
                {
                    var request = new RecetaRequest
                    {
                        ProductId = recipeResult.ProductId,
                        ProductName = recipeResult.ProductName,
                        RecipeItems = recipeResult.Items.Select(i => new RecipeItemRequest
                        {
                            IngredientId = i.Id,
                            Quantity = i.Quantity
                        }).ToList()
                    };

                    await _createRecetaUseCase.Execute(request);
                    await LoadRecetasAsync();
                }
            }
        }

        [RelayCommand]
        private async Task CreateRecipeFromProduct(RecetaModel product)
            {
                if (product == null) return;

                var newReceta = new RecetaModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Items = new List<RestaurantePOS.Core.Domain.Models.IngredientModel>()
                };

                var recipeEditorViewModel = new RecipeEditorViewModel(newReceta, _getAllIngredientesUseCase);
                var recipeResult = await _dialogService.ShowDialogAsync<RecipeEditorViewModel, RecetaModel?>(recipeEditorViewModel);

                if (recipeResult != null)
                {
                    var request = new RecetaRequest 
                    {
                        ProductId = recipeResult.ProductId,
                        ProductName = recipeResult.ProductName,
                        RecipeItems = recipeResult.Items.Select(i => new RecipeItemRequest
                        {
                            IngredientId = i.Id,
                            Quantity = i.Quantity
                        }).ToList()
                    };

                    await _createRecetaUseCase.Execute(request);
                    await LoadRecetasAsync();
                }
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
        private async Task EditRecipe(RecetaModel receta)
        {
            if (receta == null) return;

            var viewModel = new RecipeEditorViewModel(receta, _getAllIngredientesUseCase);
            var result = await _dialogService.ShowDialogAsync<RecipeEditorViewModel, RecetaModel?>(viewModel, new PixelPoint(250, 30));

            if (result is RecetaModel updatedReceta)
            {
                var request = new RecetaRequest
                {
                    ProductId = updatedReceta.ProductId,
                    ProductName = updatedReceta.ProductName,
                    RecipeItems = updatedReceta.Items.Select(i => new RecipeItemRequest
                    {
                        IngredientId = i.Id,
                        Quantity = i.Quantity
                    }).ToList()
                };

                // Log payload to help debug incorrect ingredient ids
                try
                {
                    var payload = JsonSerializer.Serialize(request);
                    Debug.WriteLine($"UpdateReceta payload: {payload}");
                    foreach (var item in request.RecipeItems)
                    {
                        Debug.WriteLine($"RecipeItem -> IngredientId: {item.IngredientId}, Quantity: {item.Quantity}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to serialize request: {ex.Message}");
                }

                // If any ingredient id is 0, try to resolve it by matching the name against AllIngredientes
                for (int idx = 0; idx < request.RecipeItems.Count; idx++)
                {
                    var ri = request.RecipeItems[idx];
                    if (ri.IngredientId == 0)
                    {
                        var match = AllIngredientes.FirstOrDefault(a => string.Equals(a.Name, updatedReceta.Items[idx].Name, StringComparison.OrdinalIgnoreCase));
                        if (match != null && match.Id > 0)
                        {
                            Debug.WriteLine($"Resolving missing IngredientId for item '{updatedReceta.Items[idx].Name}' -> {match.Id}");
                            ri.IngredientId = match.Id;
                        }
                        else
                        {
                            Debug.WriteLine($"Warning: IngredientId is 0 and no match found for '{updatedReceta.Items[idx].Name}'. Aborting update.");
                            return;
                        }
                    }
                }

                await _updateRecetaUseCase.Execute(request.ProductId.ToString(), request);
                await LoadRecetasAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteRecipe(RecetaModel receta)
        {
            if (receta == null) return;
            // TODO: Add confirmation dialog
            await _deleteRecetaUseCase.Execute(receta.ProductId.ToString());
            await LoadRecetasAsync();
        }
    }
}
