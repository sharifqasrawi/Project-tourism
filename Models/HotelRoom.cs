using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class HotelRoom
    {
        public int Id { get; set; }
        public Room Room { get; set; }


        [Display(Name = "Count", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CountReq")]
        public int Count { get; set; }

        [Display(Name = "NightPrice", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "NightPriceReq")]
        public float NightPrice { get; set; }

        public Hotel Hotel { get; set; }
    }
}