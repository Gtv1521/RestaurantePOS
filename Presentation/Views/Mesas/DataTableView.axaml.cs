using System;
using System.Collections.Specialized;
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

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is DataTableViewModel viewModel)
        {
            // Nos suscribimos a los cambios de la colección de productos pedidos
            viewModel.ProductosPedidos.CollectionChanged += ProductosPedidos_CollectionChanged;
        }
    }

    private void ProductosPedidos_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Solo si se agregó un nuevo elemento
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            // Buscamos el ListBox por su nombre en el XAML
            var listBox = this.FindControl<ListBox>("ListaPedidos");
            if (listBox != null && listBox.Items.Count > 0)
            {
                // Hacemos scroll de forma segura hasta el último ítem de la lista
                var ultimoItem = listBox.Items[listBox.Items.Count - 1];
                listBox.ScrollIntoView(ultimoItem!);
            }
        }
    }
}