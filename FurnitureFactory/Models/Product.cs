using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public class Product
    {
        public int? ID { get; set; }
        public String name { get; set; }
        public int? price { get; set; }
        public int? categoryID { get; set; }
        public int? salary { get; set; }

        public string category { get; set; }
    }

    public class prodCategories
    {
        public int ID { get; set; }
        public string name { get; set; }
    }
}