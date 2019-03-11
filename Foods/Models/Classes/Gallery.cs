using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Foods.Models.Classes
{
    public class Gallery
    {
        public int GalleryID { get; set; }
        public string Head { get; set; }
        public string Image { get; set; }
        public string Icon { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}