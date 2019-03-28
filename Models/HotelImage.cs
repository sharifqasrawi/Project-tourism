using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class HotelImage
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public Hotel Hotel { get; set; }
    }
}