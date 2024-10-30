using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _22686640_Homework_Assignment_2.Models
{
    public class User
    {         
            public string Email { get; set; }
            public string[] Roles { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
            public string ProfilePhoto { get; set; }

            public bool IsSeller { get; set; }
            public bool IsBuyer { get; set; }
                       
    }
}