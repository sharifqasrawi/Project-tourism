using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public List<FlightReservation> FlightReservations { get; set; }
        public List<FlightReservation> FlightBackReservations { get; set; }
        public List<HotelReservation> HotelReservations { get; set; }

        [Display(Name = "CustomersCount", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CustomersCountReq")]
        public int CustomersCount { get; set; }

        [Display(Name = "Discount", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DiscountReq")]
        public float Discount { get; set; }

        [Display(Name = "OldPrice", ResourceType = typeof(Resource))]
        public float Price { get; set; }
        
        [Display(Name = "NewPrice", ResourceType = typeof(Resource))]
        public float NewPrice { get; set; }

        [Display(Name = "Count", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CountReq")]
        public int Count { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_En { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_Ar { get; set; }

        [Display(Name = "Duration", ResourceType = typeof(Resource))]
        public int Duration { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string Status { get; set; }

    }
}