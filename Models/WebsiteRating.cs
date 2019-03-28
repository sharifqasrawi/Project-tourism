using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class WebsiteRating
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string Rate { get; set; }
    }
}