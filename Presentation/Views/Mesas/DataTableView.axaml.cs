using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MiComanderaApp.ViewModels.Mesas;

namespace MiComanderaApp.Views.Mesas;

public partial class DataTableView : UserControl
{
    public DataTableView()
    {
        InitializeComponent();
    }

    public void IrAlUltimoProducto(ProductoItem producto)
    {
        ProductosList.ScrollIntoView(producto);
    }
}