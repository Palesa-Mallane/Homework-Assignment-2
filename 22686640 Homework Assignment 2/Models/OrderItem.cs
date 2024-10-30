using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
    }
}