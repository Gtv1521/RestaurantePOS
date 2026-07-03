using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace MiComanderaApp.ViewModels.Orders
{
    public partial class NewOrderViewModel : ViewModelBase
    {
        public event EventHandler? OrderCreated;
        public event EventHandler? OrderFinished;


        [RelayCommand]
        public void CreateOrder()
        {
            OrderCreated?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        public void FinishOrder()
        {
            OrderFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}