using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiComanderaApp.Models;

namespace MiComanderaApp.ViewModels.Components.Products
{
    public partial class ProductViewModel : ViewModelBase
    {
        private ProductoModel? _product;

        public void Initialize(ProductoModel data)
        {
            _product = data;
        }
    }
}