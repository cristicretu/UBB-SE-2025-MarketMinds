using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLayer.ViewModel
{
    public class CreateBuyListingViewModel : CreateListingViewModelBase
    {
        public BuyProductsService BuyProductsService { get; set; }
        public override void CreateListing(Product product)
        {
            BuyProductsService.CreateListing(product);
        }
    }
}
