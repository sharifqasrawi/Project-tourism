using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;
using TravelAndTourismWebsite.Models;

namespace TravelAndTourismWebsite.Models
{
    public class ApiModelsView
    {
        public class FlightModelView
        {
            public int ID { get; set; }
            [Display(Name = "Source", ResourceType = typeof(Resource))]
            //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "SourceReq")]
            public int SourceCity { get; set; }

            [Display(Name = "Destination", ResourceType = typeof(Resource))]
            //  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DestinationReq")]
            public int DestinationCity { get; set; }

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

        public class FlightReservationsViewModel
        {
            public int ID { get; set; }

            public string Customer { get; set; }
            public Flight Flight { get; set; }

            public int FlightId { get; set; }

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

        }

        //public class HotelViewModel
        //{
        //    public int Id { get; set; }

        //    //[Display(Name = "HotelName", ResourceType = typeof(Resource))]
        //    //[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "HotelNameReq")]
        //    //public string Name_En { get; set; }

        //    //[Display(Name = "HotelName", ResourceType = typeof(Resource))]
        //    //[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "HotelNameReq")]
        //    //public string Name_Ar { get; set; }

        //    [Display(Name = "HotelName", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "HotelNameReq")]
        //    public string HotelName { get; set; }

        //    [Display(Name = "Stars", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "StarsReq")]
        //    public int Stars { get; set; }

        //    [Display(Name = "Details", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        //    public string Details_En { get; set; }

        //    [Display(Name = "Details", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        //    public string Details_Ar { get; set; }


        //    [Display(Name = "City", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CityReq")]
        //    public City City { get; set; }

        //    [Display(Name = "HotelLocation", ResourceType = typeof(Resource))]
        //    public string Location { get; set; }

        //    [Display(Name = "PhoneNumber", ResourceType = typeof(Resource))]
        //    [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PhoneNumberReq")]
        //    public string PhoneNumber { get; set; }

        //    [Display(Name = "Website", ResourceType = typeof(Resource))]
        //    public string Website { get; set; }

        //    [Display(Name = "Email", ResourceType = typeof(Resource))]
        //    [DataType(DataType.EmailAddress)]
        //    public string Email { get; set; }

        //    [Display(Name = "HotelImages", ResourceType = typeof(Resource))]
        //    public List<HotelImage> Images { get; set; }

        //    [Display(Name = "HotelRooms", ResourceType = typeof(Resource))]
        //    public List<HotelRoom> HotelRooms { get; set; }

        //    public List<HotelRate> HotelRates { get; set; }
        //}

        public class HotelRoomsModelView
        {
            public int HotelId { get; set; }
            public string Type { get; set; }
            public string DetailsTerm { get; set; }
            public float NightPriceGt { get; set; }
            public float NightPriceLs { get; set; }
            public int GuestsCount { get; set; }

        }
        public class HotelReservationModelView
        {
            public int RoomId { get; set; }
            public int Id { get; set; }

            [Display(Name = "Guest", ResourceType = typeof(Resource))]
            public string Guest { get; set; }
            public HotelRoom Room { get; set; }

            [Display(Name = "Check_In_Date", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Check_In_DateReq")]
            public DateTime Check_In_Date { get; set; }

            public string DisplayCheck_In_Date { get; set; }

            [Display(Name = "Check_Out_Date", ResourceType = typeof(Resource))]
            [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Check_Out_DateReq")]
            public DateTime Check_Out_Date { get; set; }

            public string DisplayCheck_Out_Date { get; set; }

            [Display(Name = "ReservationCost", ResourceType = typeof(Resource))]
            public float ReservationCost { get; set; }
            public int RoomsAvailable { get; set; }
        }

        public class HotelByNameModelView 
        {
            public int Country;
            public int City;
            public string HotelName;
        }

        public class OfferReservationModelView
        {
            //public int OfferId;
            //public string Customer;

            public int Id { get; set; }
            public string Customer { get; set; }
            public Offer Offer { get; set; }
            public DateTime DateTime { get; set; }

        }


        public class HotelRateModelView
        {
            public int Id { get; set; }
            public string User { get; set; }
            public int HotelId { get; set; }
            public string Rate { get; set; }
        }

        public class MessageModelView
        {
            public int Id { get; set; }

            public string Email { get; set; }

            public string Subject { get; set; }

            public string Text { get; set; }

            public DateTime Date_Time { get; set; }

            public string DisplayDate_Time { get; set; }

            public bool IsRead { get; set; }
        }

        public class CreditCardViewModel
        {
            public int Id { get; set; }

            public string CardType { get; set; }

            public string NameOnCard { get; set; }

            public string CardNumber { get; set; }

            public string CardVerificationCode { get; set; }

            public string CardExpirationDate { get; set; }
            public string UserId { get; set; }
        }

        public class RechargeViewModel
        {
            public CreditCardViewModel CreditCard { get; set; }
            public float CurrentCredit { get; set; }
            public float NewCredit { get; set; }
            public string UserId { get; set; }

        }

        public class LoginViewModel
        {

            public string UserName { get; set; }

            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public class UpdateAccountViewModel
        {

            public string UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Country { get; set; }
            public string City { get; set; }

        }

        public class ChangePasswordViewModel
        {
            //  public string Email { get; set; }
            public string UserId { get; set; }
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class ResetPasswordViewModel
        {
           public string Email { get; set; }
           
        }
    }
}
