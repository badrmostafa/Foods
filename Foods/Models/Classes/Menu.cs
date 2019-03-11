using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class Menu
    {
        public int MenuID { get; set; }
        public string Head { get; set; }
        public string Head1 { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        //NavigationProperty
        public List<Client> Clients { get; set; }
    }
}