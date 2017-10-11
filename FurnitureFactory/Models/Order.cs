using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int productID { get; set; }
        public int purchaserID { get; set; }
        public int price { get; set; }
        public DateTime date { get; set; }
        public int? orderPackageID { get; set; }
        public bool? state { get; set; }
        public int number { get; set; }

        public string purchaser { get; set; }
        public string product { get; set; }
    }
}