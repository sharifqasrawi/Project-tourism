using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class CityImage
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public City City { get; set; }
    }
}