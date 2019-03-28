using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class HomeViewModel
    {
        public List<Country> Countries { get; set; }
        public List<Hotel> Hotels { get; set; }
        public List<Offer> Offers { get; set; }
    }
}