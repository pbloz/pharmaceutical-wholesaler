using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public String Description { get; set; }
        public String UserName { get; set; }
        public int ProductComId { get; set; }
    }
}