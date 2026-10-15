using System.Threading.Tasks;
using MiComanderaApp.Core.Infrastructure.Api;

namespace RestaurantePOS.Core.Application.UseCases.Ingrediente
{
    public class AjustarStockUseCase
    {
        private readonly StockRepository _repo;
        public AjustarStockUseCase(StockRepository repo) => _repo = repo;

        public Task<bool> Execute(int ingredientId, decimal newQuantity, string reason)
            => _repo.AjustarAsync(ingredientId, newQuantity, reason);
    }
}
