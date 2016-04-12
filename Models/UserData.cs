﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class UserData
    {
        //[Required(ErrorMessage = "Wprowadź imię")]
        //[MaxLength(50), MinLengthAttribute(1)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Wprowadź nazwisko")]
        //[MaxLength(50), MinLengthAttribute(1)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Wprowadź adres zamieszkania")]
        //[MaxLength(100), MinLengthAttribute(1)]
        [Display(Name = "Adres zamieszkania")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Wprowadź kod pocztowy i miasto")]
        //[MaxLength(30), MinLengthAttribute(1)]
        [Display(Name = "Kod pocztowy i miasto")]
        public string CodeAndCity { get; set; }

        [RegularExpression(@"(\+\d{2})*[\d\s-]+",
            ErrorMessage = "Błędny format numeru telefonu")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Błędny format adresu e-mail")]
        [Required(ErrorMessage = "Nie wprowadzono adresu e-mail")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Wprowadź nazwę firmy")]
        [Display(Name = "Nazwa firmy")]
        public string Factory { get; set; }

        //[Required(ErrorMessage = "Wprowadź  NIP")]
        [Display(Name = "Nip")]
        [RegularExpression(@"^((\d{3}[- ]\d{3}[- ]\d{2}[- ]\d{2})|(\d{3}[- ]\d{2}[- ]\d{2}[- ]\d{3}))$",
            ErrorMessage = "Błędny format numeru nip")]
        public string Nip { get; set; }

        public string FirstNameAndLastName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}