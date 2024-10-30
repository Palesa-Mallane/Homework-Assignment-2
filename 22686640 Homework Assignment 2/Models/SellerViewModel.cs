using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class SellerViewModel
    {
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string ProfilePhoto { get; set; }
        public string StoreLocation { get; set; }
        public List<Product> ProductsListed { get; set; }
    }
}