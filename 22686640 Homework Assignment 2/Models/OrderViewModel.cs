using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class OrderViewModel
    {
        
            public int StoreId { get; set; }
            public int StaffId { get; set; }
            public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public class OrderItemViewModel
        {
            public int ItemId { get; set; } // Unique ID for the item in this order
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal ListPrice { get; set; }
            public decimal Discount { get; set; }
        }

    }
}