//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAnPhanMem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductImg
    {
        public int product_img_id { get; set; }
        public int product_id { get; set; }
        public string image { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
