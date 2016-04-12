using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Podaj nazwę kategorii")]
        //[MaxLength(20),MinLengthAttribute(1)]
        [Display(Name = "Nazwa kategorii")]
        public string NameCategory { get; set; }
        [Required(ErrorMessage = "Podaj nazwę kategorii")]
        //[MaxLength(50), MinLengthAttribute(1)]
        [Display(Name = "Opis")]
        public string Description { get; set; }
        //ścieżka do pliku strony z nazwą kategorii
        [Required(ErrorMessage="Ikona kategorii jest wymagana")]
        public string IconFilename { get; set; }

        //właściwość nawigacyjna do kolekcji albumów z danej kategori
        public virtual ICollection<Product> Products { get; set; }
    }
}