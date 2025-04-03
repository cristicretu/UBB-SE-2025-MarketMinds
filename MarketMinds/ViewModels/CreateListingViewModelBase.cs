using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public abstract class CreateListingViewModelBase
    {
        public string Title { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductTag> Tags { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public ProductCondition Condition { get; set; }

        public string ImagesString
        {
            get => Images != null ? string.Join("\n", Images.Select(img => img.Url)) : string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Images = value.Split("\n").Select(url => new Image(url)).ToList();
                }
                else
                {
                    Images = new List<Image>();
                }
            }
        }

        public void CreateTag(string tag)
        {
            if (Tags == null)
            {
                Tags = new List<ProductTag>();
            }
            Tags.Add(new ProductTag(-1, tag));
        }

        public abstract void CreateListing(Product product);
    }
}

