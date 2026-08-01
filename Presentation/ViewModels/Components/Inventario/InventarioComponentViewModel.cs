using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Application.UseCases.Inventario.Ingredientes;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Presentation.ViewModels.Components.Inventario
{
    public partial class InventarioComponentViewModel : ViewModelBase
    {
        private readonly GetAllIngredientesUseCase _getAllIngredientUseCase;

        public ObservableCollection<IngredienteModel> Ingredients { get; } = new();

        public InventarioComponentViewModel(GetAllIngredientesUseCase getAllIngredientUseCase)
        {
            _getAllIngredientUseCase = getAllIngredientUseCase;
            _ = LoadIngredients();
        }

        [RelayCommand]
        private async Task LoadIngredients()
        {
            Ingredients.Clear();
            var ingredients = await _getAllIngredientUseCase.ExecuteAsync();
            System.Console.WriteLine($"Número de ingredientes recibidos: {ingredients.Count()}");
            foreach (var ingredient in ingredients)
            {
                Ingredients.Add(ingredient);
            }
        }

        [RelayCommand]
        private void LoadRecipes()
        {
            // Placeholder for loading recipes
        }
        
        [RelayCommand]
        private void NewRecipe()
        {
            // Placeholder for creating a new recipe
        }

        [RelayCommand]
        private void NewIngredient()
        {
            // Placeholder for creating a new ingredient
        }

        [RelayCommand]
        private void PrintInventory()
        {
            // Placeholder for printing the inventory
        }
    }
}
