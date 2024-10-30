using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class ProductViewModel
    {    
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int ModelYear { get; set; }
        public decimal ListPrice { get; set; }
        public string ImageURL { get; set; }

        public string StoreName { get; set; }

        public int Quantity { get; set; }

        public DateTime TransactionDate { get; set; } // Date of the transaction
        
        public string SellerImage {  get; set; }

        public string SellerName { get; set; }
        public string SellerPhone { get; set; }
        public string SellerEmail { get; set; }

       



    }
}