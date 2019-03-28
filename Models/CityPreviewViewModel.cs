using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class CityPreviewViewModel
    {
        public City City { get; set; }
        public List<Flight> Flights { get; set; }
        public List<Hotel> Hotels { get; set; }
    }
}