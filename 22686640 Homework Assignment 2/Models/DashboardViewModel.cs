using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class DashboardViewModel
    {
        public List<ProductViewModel> BoughtProducts { get; set; }
        public List<ProductViewModel> SoldProducts { get; set; }
    }
}