using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services;

public class BorrowProductsService : ProductService
{
    private BorrowProductsRepository borrowProductsRepository;

    public BorrowProductsService(BorrowProductsRepository repository): base(repository)
    {
        this.borrowProductsRepository = repository;
    }

    public List<Product> GetAllProducts()
    {
        return borrowProductsRepository.GetProducts();
    }
}