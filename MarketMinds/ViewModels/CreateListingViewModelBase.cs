using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services;

namespace ViewModelLayer.ViewModel
{
    public abstract class CreateListingViewModelBase
    {
        private const int TAGID = -1;

        public string Title { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductTag> Tags { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public ProductCondition Condition { get; set; }

        private readonly ImageUploadService imageService;

        public CreateListingViewModelBase()
        {
            imageService = new ImageUploadService();
            Images = new List<Image>();
            Tags = new List<ProductTag>();
        }

        public string ImagesString
        {
            get => imageService.FormatImagesString(Images);
            set => Images = imageService.ParseImagesString(value);
        }

        public void AddTag(string tag)
        {
            if (Tags == null)
            {
                Tags = new List<ProductTag>();
            }

            Tags.Add(new ProductTag(TAGID, tag));
        }

        public abstract void CreateListing(Product product);
    }
}
