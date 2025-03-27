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
        try
        {
            System.Diagnostics.Debug.WriteLine("BuyProductsService.CreateListing started");
            buyProductsRepository.AddProduct(product);
            System.Diagnostics.Debug.WriteLine("BuyProductsService.CreateListing ended");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in BuyProductsService.CreateListing: {ex.Message}");
        }
    }

    public void DeleteListing(Product product)
    {
        buyProductsRepository.DeleteProduct(product);
    }
}