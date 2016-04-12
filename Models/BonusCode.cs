using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class BonusCode
    {
        public int BonusCodeId { get; set; }
        [Required(ErrorMessage = "Nie wprowadzono kodu rabatowego")]
        [MaxLength(10), MinLengthAttribute(10)]
        [Display(Name="Kod rabatowy")]
        public String Code { get; set; }

    }
}