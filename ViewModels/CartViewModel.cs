using Hurtownia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public List<CompareItem> CompareItems { get; set; }

        public decimal TotalPrice { get; set; }

    }
}