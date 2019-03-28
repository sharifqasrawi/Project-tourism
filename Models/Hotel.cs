using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Display(Name = "HotelName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "HotelNameReq")]
        public string Name_En { get; set; }

        [Display(Name = "HotelName", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "HotelNameReq")]
        public string Name_Ar { get; set; }

        [Display(Name = "Stars", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "StarsReq")]
        public int Stars { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_En { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_Ar { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CityReq")]
        public City City { get; set; }

        [Display(Name = "HotelLocation", ResourceType = typeof(Resource))]
        public string Location { get; set; }

        [Display(Name = "GpsX", ResourceType = typeof(Resource))]
        public string GpsX { get; set; }
        [Display(Name = "GpsY", ResourceType = typeof(Resource))]
        public string GpsY { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PhoneNumberReq")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Website", ResourceType = typeof(Resource))]
        public string Website { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "HotelImages", ResourceType = typeof(Resource))]
        public List<HotelImage> Images { get; set; }

        [Display(Name = "HotelRooms", ResourceType = typeof(Resource))]
        public List<HotelRoom> HotelRooms { get; set; }

        public List<HotelRate> HotelRates { get; set; }
        public int Rate { get; set; }
    }
}