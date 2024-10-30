﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class Seller
    {
        public int SellerID { get; set; }
        public string Name { get; set; }           
              
        public string Email { get; set; }      
        
             
        public string Phone { get; set; }       
        public string Street { get; set; }      
        public string City { get; set; }       
        public string State { get; set; }      
        public string ZipCode { get; set; }

        public string SelectedStore {  get; set; }
        public string ProfilePhoto { get; set; }
        
    }
}