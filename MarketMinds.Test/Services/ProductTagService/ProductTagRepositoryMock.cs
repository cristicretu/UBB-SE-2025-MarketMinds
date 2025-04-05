using DomainLayer.Domain;
using MarketMinds.Repositories.ProductTagRepository;
using System.Collections.Generic;
using System.Linq;

namespace MarketMinds.Test.Services.ProductTagService
{
    internal class ProductTagRepositoryMock : IProductTagRepository
    {
        // Storage for mock data
        public List<ProductTag> Tags { get; set; }
        private int currentId = 0;

        public ProductTagRepositoryMock()
        {
            Tags = new List<ProductTag>();
        }

        public List<ProductTag> GetAllProductTags()
        {
            return Tags;
        }

        public ProductTag CreateProductTag(string displayTitle)
        {
            var newTag = new ProductTag(++currentId, displayTitle);
            Tags.Add(newTag);
            return newTag;
        }

        public void DeleteProductTag(string displayTitle)
        {
            Tags.RemoveAll(t => t.DisplayTitle == displayTitle);
        }
    }
}