using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModelLayer.ViewModel;

namespace ViewModelLayer.ViewModel
{
    public class CreateListingViewModel
    {
        public CreateListingViewModelBase ViewModel { get; set; }
        public void SwitchViewModel(string listingType)
        {
            switch (listingType)
            {
                case "buy":
                    ViewModel = new CreateBuyListingViewModel();
                    break;
                case "borrow":
                    ViewModel = new CreateBorrowListingViewModel();
                    break;
                case "auction":
                    ViewModel = new CreateAuctionListingViewModel();
                    break;
            }
        }
    }
}
