using System;
using DomainLayer.Domain;

namespace MarketMinds.Services
{
    public class ProductPriceService
    {
        public float CalculateBorrowPrice(BorrowProduct product, DateTime startDate, DateTime endDate)
        {
            TimeSpan duration = endDate - startDate;
            int days = duration.Days + 1; // Include both start and end dates
            return days * product.DailyRate;
        }

        public string FormatPrice(float price)
        {
            return price.ToString("C"); // Format as currency
        }
    }
} 