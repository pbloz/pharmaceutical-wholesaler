using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class Manufacture
    {
        public int ManufactureId { get; set; }

        [Display(Name = "Nazwa firmy")]
        public string Name { get; set; }

        [MaxLength(100), MinLengthAttribute(1)]
        [Display(Name = "Adres producenta")]
        public string Address { get; set; }

        [MaxLength(30), MinLengthAttribute(1)]
        [Display(Name = "Kod pocztowy i miasto")]
        public string CodeAndCity { get; set; }


        [RegularExpression(@"(\+\d{2})*[\d\s-]+",
            ErrorMessage = "Błędny format numeru telefonu")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Nip")]
        [RegularExpression(@"^((\d{3}[- ]\d{3}[- ]\d{2}[- ]\d{2})|(\d{3}[- ]\d{2}[- ]\d{2}[- ]\d{3}))$",
            ErrorMessage = "Błędny format numeru nip")]
        public string Nip { get; set; }
        //public virtual ICollection<Product> Products { get; set; }

    }
}