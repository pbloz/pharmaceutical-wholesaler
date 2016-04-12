using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hurtownia.Models;

namespace Hurtownia.ViewModels
{
    public class ReferToFriendHelper
    {
        public Product Product { get; set; }
        public string Title { get; set; }
        public string EmailReceiver { get; set; }
        public string Text { get; set; }
        public string Autor { get; set; }
    }
}