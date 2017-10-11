using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public class Money
    {
        public int ID { get; set; }
        public float change { get; set; }
        public DateTime date { get; set; }
        public string note { get; set; }
        public int? material_regID { get; set; }
        public int? orderID { get; set; }
        public int? workgroupID { get; set; }
    }
}