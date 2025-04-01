using System.Collections.Generic;
using DomainLayer.Domain;

/// <summary>
/// Interface for managing products in the service layer.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    List<Product> GetProducts();

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to add.</param>
    void AddProduct(Product product);

    /// <summary>
    /// Deletes an existing product.
    /// </summary>
    /// <param name="product">The product to delete.</param>
    void DeleteProduct(Product product);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product with updated information.</param>
    void UpdateProduct(Product product);
}

