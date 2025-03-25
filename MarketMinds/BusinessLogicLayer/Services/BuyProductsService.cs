using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services;

public class BuyProductsService: ProductService
{
    private BuyProductsRepository buyProductsRepository;

    public BuyProductsService(BuyProductsRepository repository): base(repository)
    {
        this.buyProductsRepository = repository;
    }
    
    public void CreateListing(Product product)
    {
        buyProductsRepository.AddProduct(product);
    }

    public void DeleteListing(Product product)
    {
        buyProductsRepository.DeleteProduct(product);
    }
}