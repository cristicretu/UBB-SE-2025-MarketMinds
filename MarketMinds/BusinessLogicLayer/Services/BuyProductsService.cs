using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services;

public class BuyProductsService
{
    private BorrowProductsRepository buyProductsRepository;

    public BuyProductsService(BorrowProductsRepository borrowProductsRepository)
    {
        this.buyProductsRepository = borrowProductsRepository;
    }
    
    public List<Product> GetAllProducts()
    {
        return buyProductsRepository.GetProducts();
    }
}