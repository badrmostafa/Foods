using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public string Head { get; set; }
        public int ClientID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        //NavigationProperty
        public Client Client { get; set; }
    }
}