using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class HotelRate
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public Hotel Hotel { get; set; }
        public string Rate { get; set; }
    }
}