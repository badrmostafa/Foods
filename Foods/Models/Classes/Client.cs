using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class Client
    {
        public int ClientID { get; set; }
        public string Image { get; set; }
        public string Head { get; set; }
        public string Head1 { get; set; }
        public string Description { get; set; }
        public int MenuID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        //NavigationProperty
        public Menu Menu { get; set; }

    }
}