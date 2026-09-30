using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiComanderaApp.Interfaces;
using MiComanderaApp.Models;
using MiComanderaApp.Presentation.Views.Components.Generales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiComanderaApp.ViewModels.Dialogs.Modals
{
    public partial class SelectProductForRecipeViewModel : ViewModelBase, MiComanderaApp.Core.Domain.Interfaces.IDialogViewModel<MiComanderaApp.Models.ProductoModel?>
    {
        private readonly IViewModelFactory _factory;

        [ObservableProperty]
        private string _title = "Seleccionar Producto";

        [ObservableProperty]
        private bool _teclado;

        [ObservableProperty]
        private bool _botonTeclado = true;

        [ObservableProperty]
        private object? _vistaActual;
        [ObservableProperty]
        private ObservableCollection<ProductoModel> _products;

        [ObservableProperty]
        private ProductoModel? _selectedProduct;

        [ObservableProperty]
        private string _searchText = string.Empty;

        private readonly List<ProductoModel> _allProducts = new();

        public event Action<ProductoModel?>? CloseRequested;

        public SelectProductForRecipeViewModel(IEnumerable<ProductoModel> products, IViewModelFactory factory)
        {
            _factory = factory;
            _vistaActual = _factory.Create<TecladoComponentViewModel>();
            _allProducts.AddRange(products);
            _products = new ObservableCollection<ProductoModel>(_allProducts);
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterProducts(value);
        }

        private void FilterProducts(string? filter)
        {
            var lower = (filter ?? string.Empty).Trim();
            var filtered = string.IsNullOrWhiteSpace(lower)
                ? _allProducts
                : _allProducts.Where(p => p.Name.Contains(lower, StringComparison.OrdinalIgnoreCase) || (p.CategoryName?.Contains(lower, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

            Products.Clear();
            foreach (var p in filtered)
                Products.Add(p);
        }

        [RelayCommand]
        private void Select()
        {
            CloseRequested?.Invoke(SelectedProduct);
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
