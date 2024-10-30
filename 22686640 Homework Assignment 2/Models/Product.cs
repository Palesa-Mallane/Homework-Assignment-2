using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public DateTime ModelYear { get; set; }
        public decimal ListPrice { get; set; }
        public string ImageURL { get; set; }

        public int SellerId { get; set; }
        public int StoreId { get; set; }

        public DateTime ListingDate { get; set; }


    }
}