using Hurtownia.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.ViewModels
{
    public class CategoryStatistics
    {
        
        public int Categorys { get; set; }

        public String CategoryName { get; set; }
    }
}