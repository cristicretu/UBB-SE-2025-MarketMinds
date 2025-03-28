﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class CreateBorrowListingViewModel : CreateListingViewModelBase
    {
        public BorrowProductsService BorrowProductsService { get; set; }
        public override void CreateListing(Product product)
        {
            BorrowProductsService.CreateListing(product);
        }
    }
}
