using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class Slider
    {
        public int SliderId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}