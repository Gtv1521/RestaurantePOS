using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Core.Domain.Models;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Presentation.Views.Components.Generales;
using RestaurantePOS.Core.Domain.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using RestaurantePOS.Core.Application.UseCases.Ingrediente;
using System;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class RecipeEditorViewModel : ViewModelBase, MiComanderaApp.Core.Domain.Interfaces.IDialogViewModel<RecetaModel?>
    {
        private readonly GetAllIngredientesUseCase _getAllIngredientesUseCase;
        private readonly IViewModelFactory _factory;

        [ObservableProperty]
        private string _title = "Editar Receta";

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;
        [ObservableProperty]
        private RecetaModel _recipe;

        [ObservableProperty]
        private ObservableCollection<IngredientModel> _recipeIngredients;

        [ObservableProperty]
        private ObservableCollection<IngredienteModel> _allIngredients;

        [ObservableProperty]
        private IngredienteModel? _selectedIngredient;

        [ObservableProperty]
        private double _quantity;

        public event Action<RecetaModel?>? CloseRequested;

        public RecipeEditorViewModel(RecetaModel recipe, GetAllIngredientesUseCase getAllIngredientesUseCase, IViewModelFactory factory)
        {
            _recipe = recipe;
            _getAllIngredientesUseCase = getAllIngredientesUseCase;
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _recipeIngredients = new ObservableCollection<IngredientModel>(recipe.Items);
            _allIngredients = new ObservableCollection<IngredienteModel>();
            _ = LoadAllIngredients();
        }

        private async Task LoadAllIngredients()
        {
            var ingredients = await _getAllIngredientesUseCase.Execute();
            foreach (var ingredient in ingredients)
            {
                AllIngredients.Add(ingredient);
            }
        }

        [RelayCommand]
        private void AddIngredient()
        {
            if (SelectedIngredient != null && Quantity > 0)
            {
                var existingIngredient = RecipeIngredients.FirstOrDefault(i => i.Id == SelectedIngredient.Id);
                if (existingIngredient != null)
                {
                    existingIngredient.Quantity = Quantity;
                }
                else
                {
                    var newIngredient = new IngredientModel
                    {
                        Id = SelectedIngredient.Id,
                        Name = SelectedIngredient.Name,
                        Quantity = Quantity,
                        UnitOfMeasure = SelectedIngredient.UnitOfMeasure
                    };
                    RecipeIngredients.Add(newIngredient);
                }
                
                // Reset fields
                SelectedIngredient = null;
                Quantity = 0;
            }
        }

        [RelayCommand]
        private async Task RemoveIngredient(IngredientModel ingredient)
        {
            if (ingredient != null)
            {
                RecipeIngredients.Remove(ingredient);
            }
        }

        [RelayCommand]
        private void Save()
        {
            Recipe.Items = new List<IngredientModel>(RecipeIngredients);
            CloseRequested?.Invoke(Recipe);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseRequested?.Invoke(null);
        }

        [RelayCommand]
        private void VerTeclado()
        {
            Teclado = !Teclado;
            BotonTeclado = !BotonTeclado;
        }
    }
}
