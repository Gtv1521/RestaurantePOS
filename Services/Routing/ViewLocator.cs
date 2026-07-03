using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MiComanderaApp.ViewModels;

namespace MiComanderaApp.Services.Routing
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null) return null;

            // 1. Obtenemos el nombre completo de la clase del ViewModel
            // Ejemplo: "MiComanderaApp.ViewModels.Orders.NewOrderViewModel"
            var name = data.GetType().FullName!;

            // 2. Reemplazamos la palabra "ViewModel" por "View"
            // Resultado: "MiComanderaApp.Views.Orders.NewOrderView"
            name = name.Replace("ViewModel", "View");

            // 3. Intentamos buscar el tipo de la Vista en el ensamblado
            var type = Type.GetType(name);

            if (type != null)
            {
                // 🚀 Si lo encuentra, crea la interfaz gráfica dinámicamente
                return (Control)Activator.CreateInstance(type)!;
            }

            // Si no lo encuentra, te dibuja un texto de error para avisarte qué falta
            return new TextBlock { Text = $"No se encontró la vista para: {name}" };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}