using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Flight
    {
        public int ID { get; set; }
        [Display(Name = "Source", ResourceType = typeof(Resource))]
    //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "SourceReq")]
        public City SourceCity { get; set; }
        
        [Display(Name = "Destination", ResourceType = typeof(Resource))]
      //  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DestinationReq")]
        public City DestinationCity { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DateReq")]
        public DateTime Date { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Resource))]
        public string DisplayDate { get; set; }

        [Display(Name = "Time", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "TimeReq")]
        public string Time { get; set; }

        [Display(Name = "EcoSeatsCount", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EcoSeatsCountReq")]
        public int EcoSeatsCount { get; set; }

        [Display(Name = "FrstSeatsCount", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FrstSeatsCountReq")]
        public int FrstSeatsCount { get; set; }

        [Display(Name = "FlightDuration", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FlightDurationReq")]
        public string FlightDuration { get; set; }

        [Display(Name = "EconomyTicketPrice", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EconomyTicketPriceReq")]
        public double EconomyTicketPrice { get; set; }

        [Display(Name = "FirstClassTicketPrice", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FirstClassTicketPriceReq")]
        public double FirstClassTicketPrice { get; set; }

        [Display(Name = "Airline", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FirstClassTicketPriceReq")]
        public string Airline { get; set; }

        public List<FlightReservation> Reservations { get; set; }
    }
}