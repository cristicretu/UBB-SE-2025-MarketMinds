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

    public void CreateListing(Product product)
    {
        borrowProductsRepository.AddProduct(product);
    }

    public void DeleteListing(Product product)
    {
        borrowProductsRepository.DeleteProduct(product);
    }
}