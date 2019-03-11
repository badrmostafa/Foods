using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class About
    {
        public int AboutID { get; set; }
        public string Head { get; set; }
        public string Head1 { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}