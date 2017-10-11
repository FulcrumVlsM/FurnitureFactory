using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FurnitureFactory.Models
{
    public class Material
    {
        public int ID { get; set; }
        public string name { get; set; }
        public float reserve { get; set; }
    }

    public class MaterialReg
    {
        public int ID { get; set; }
        public string name { get; set; }
        public float number { get; set; }
        public DateTime date { get; set; }
        public float? money { get; set; }
    }
}