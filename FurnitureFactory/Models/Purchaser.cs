using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public class Purchaser
    {
        public int ID { get; set; }
        public String name { get; set; }
        public string email { get; set; }
        public string tel_number { get; set; }
        public string other_contacts { get; set; }
        public string username { get; set; }
        public string passwd { get; set; }
        public int OrderCount { get; set; }
    }
}