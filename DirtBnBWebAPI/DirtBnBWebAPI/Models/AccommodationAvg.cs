using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtBnBWebAPI.Models
{
    public class AccommodationAvg
    {
        public string city { get; set; }
        public string province { get; set; }
        public long avgAccommodationPrice { get; set; }
    }
}