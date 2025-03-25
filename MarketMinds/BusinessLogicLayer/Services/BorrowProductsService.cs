using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services;

public class BorrowProductsService
{
    private BorrowProductsRepository borrowProductsRepository;

    public BorrowProductsService(BorrowProductsRepository borrowProductsRepository)
    {
        this.borrowProductsRepository = borrowProductsRepository;
    }

    public List<Product> GetAllProducts()
    {
        return borrowProductsRepository.GetProducts();
    }
}