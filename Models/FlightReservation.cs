using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class FlightReservation
    {
        public int ID { get; set; }

        [Display(Name = "Passenger", ResourceType = typeof(Resource))]
        public ApplicationUser Customer { get; set; }
        public Flight Flight { get; set; }

        [Display(Name = "SeatsCount", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "SeatsCountReq")]
        public int Seats { get; set; }
      
        [Display(Name = "FlightClass", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FlightClassReq")]
        public string FlightClass { get; set; }

        [Display(Name = "ReservationPrice", ResourceType = typeof(Resource))]
        public double ReservationPrice { get; set; }

        [Display(Name = "DateTime", ResourceType = typeof(Resource))]
        public DateTime DateTime { get; set; }

        [Display(Name = "DateTime", ResourceType = typeof(Resource))]
        public string DisplayDateTime { get; set; }

        public bool IsBooked { get; set; }

    }
}