using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class Food
    {
       
        public int FoodID { get; set; }
        public string Head { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}