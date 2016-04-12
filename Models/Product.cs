using DataAnnotationsExtensions;
using Foolproof;
using Hurtownia.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class Product
    {
        //klucz glowny
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Nie wprowadzono nazwy produktu.")]
        [MinLengthAttribute(1)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Nie wprowadzono opisu")]
        public string ProductDescription { get; set; }
        //public DateTime DateAdded { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MMMM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",
               ApplyFormatInEditMode = true)]
        [Display(Name = "Data dodania")]
        private DateTime dateAdded;

        public DateTime DateAdded
        {
            get { return DateTime.Now; }
            set { dateAdded = DateTime.Now; }
        }

        [Display(Name = "Data ważności")]
        [DataType(DataType.Time)]
        [DisplayFormatAttribute(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        //[ValidBirthDate(ErrorMessage = "Birth Date can not be greater than current date")]
        //private DateTime dateEnd = DateTime.Now;

        //[DateRange("01/01/3000",ErrorMessage="Niewłaściwa data.")]
        [CurrentDateAttribute()]
        public DateTime DateEnd { get; set; }

        //public DateTime DateEnd { get; set; } 

        //DateTime now = DateTime.Now;
        // static TimeSpan difference = now - dateAdded;
        //[Required(ErrorMessage = "Nie wprowadzono ceny")]
        [Range(0.01, 1000.00, ErrorMessage = "Cena musi być pomiędzy 0.01 and 1000.00")]
        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }

        //nazwa pliku okładki
        public string CoverFileName { get; set; }

        //najlepiej sprzedajacy
        public bool IsBestseller { get; set; }
        //ukryty czy nie do wyświetlania zamówień z przed kilku lat true-nie będzie widoczny
        public bool IsHidden { get; set; }
        public bool Prescription { get; set; }

        public int Rating { get; set; }

        public String Form { get; set; }

        public String Components { get; set; }

        public String Contraindications { get; set; }

        //klucz obcy
        public int CategoryId { get; set; }

        public int ManufactureId { get; set; }
        public List<Comment> Comments { get; set; }

        public virtual Category Category { get; set; }
        public virtual Manufacture Manufacture { get; set; }

    }
}