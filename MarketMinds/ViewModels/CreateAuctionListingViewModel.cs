using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using BusinessLogicLayer.Services;

namespace ViewModelLayer.ViewModel
{
    public class CreateAuctionListingViewModel : CreateListingViewModelBase
    {
        public AuctionProductsService auctionProductsService { get; set; }
        public override void CreateListing(Product product)
        {
            auctionProductsService.AddProduct(product);
        }
    }
}
