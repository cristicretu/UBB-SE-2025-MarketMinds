using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class CompareProductsViewModel
    {
        public Product LeftProduct;
        public Product RightProduct;

        public CompareProductsViewModel()
        {
        }

        public bool AddProduct(Product product)
        {
            if (LeftProduct == null)
            {
                LeftProduct = product;
                return false;
            }
            if (product != LeftProduct)
            {
                RightProduct = product;
            }
            return true;
        }
    }
}