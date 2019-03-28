using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class OfferReservation
    {
        public int Id { get; set; }
        public ApplicationUser Customer { get; set; }
        public Offer Offer { get; set; }
        public DateTime DateTime { get; set; }
    }
}