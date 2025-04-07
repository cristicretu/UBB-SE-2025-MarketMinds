using DomainLayer.Domain;

namespace MarketMinds.Services
{
    public class SortTypeConverterService
    {
        public ProductSortType? Convert(string sortTag)
        {
            switch (sortTag)
            {
                case "PriceAsc":
                    return new ProductSortType("Price", "Price", true);
                case "PriceDesc":
                    return new ProductSortType("Price", "Price", false);
                default:
                    return null;
            }
        }
    }
}