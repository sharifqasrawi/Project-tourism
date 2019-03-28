using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class HotelReservation
    {
        public int Id { get; set; }

        [Display(Name = "Guest", ResourceType = typeof(Resource))]
        public ApplicationUser Guest { get; set; }
        public HotelRoom Room { get; set; }

        [Display(Name = "Check_In_Date", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Check_In_DateReq")]
        public DateTime Check_In_Date { get; set; }

        public string DisplayCheck_In_Date { get; set; }

        [Display(Name = "Check_Out_Date", ResourceType = typeof(Resource))]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Check_Out_DateReq")]
        public DateTime Check_Out_Date { get; set; }

        public string DisplayCheck_Out_Date { get; set; }

        [Display(Name = "ReservationCost", ResourceType = typeof(Resource))]
        public float ReservationCost { get; set; }
        public int RoomsAvailable { get; set; }
        public bool IsBooked { get; set; }
    }
}