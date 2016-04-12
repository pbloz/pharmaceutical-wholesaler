using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //właściwość nawigacyjna do pobierania informacji
        public virtual Product Product { get; set; }
        public virtual Order Order { get; set; }
        
    }
}