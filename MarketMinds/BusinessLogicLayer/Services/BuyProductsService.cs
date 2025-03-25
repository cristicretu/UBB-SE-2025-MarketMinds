using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services;

public class BuyProductsService: ProductService
{
    private BorrowProductsRepository buyProductsRepository;

    public BuyProductsService(BorrowProductsRepository repository): base(repository)
    {
        this.buyProductsRepository = repository;
    }
    
    public List<Product> GetAllProducts()
    {
        return buyProductsRepository.GetProducts();
    }
}