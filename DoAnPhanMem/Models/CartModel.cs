using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DoAnPhanMem.Models;

namespace DoAnPhanMem.Models
{
    public class CartModel
    {
        WebshopEntities database = new WebshopEntities();
        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public string pro_img { get; set; }
        public int discount_id { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public string status_ { get; set; }
        public bool discount { get; set; }
        public virtual Discount Discount { get; set; }
    }
}