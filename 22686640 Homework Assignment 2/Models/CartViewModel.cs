using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class CartViewModel
    {
        public List<ProductViewModel> CartProducts { get; set; }

        public decimal CartTotal
        {
            get
            {
                return CartProducts.Sum(item => item.ListPrice * item.Quantity);
            }
        }
    }
}