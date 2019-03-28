using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.WebsiteLanguages;
using TravelAndTourismWebsite.Resources;
using System.Security.Cryptography;
using System.Text;
using TravelAndTourismWebsite.Repository;

namespace TravelAndTourismWebsite.Repos
{
    internal interface IHotels
    {
        List<HotelReservation> Guest_reservation_hotels(string guestId);
        HotelReservation theGuest_reservation(int reservationId);
        Hotel searchHotelRooms(int HotelId, string Type, int GuestsCount,
                        string DetailsTerm, float NightPriceGt, float NightPriceLs);
        //  HotelReservation reserveRoom(HotelReservation model, int RoomId);

    }
    public class PostHotels : IHotels
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public List<HotelReservation> Guest_reservation_hotels(string guestId)
        {
            List<HotelReservation> hotelRes = db.HotelReservations
                                     .Include("Room.Hotel")
                                     .Include("Room")
                                     .Include("Room.Room")
                                     .Include("Guest")
                                     .Include("Room.Hotel.City")
                                     .Include("Room.Hotel.City.Country")
                                     .Include("Room.Hotel.Images")
                                     .Where(x => x.Guest.Id == guestId
                                      && ((x.Check_In_Date > DateTime.Now) || (x.Check_Out_Date < DateTime.Now)))
                                     .ToList();
            return hotelRes == null ? null : hotelRes;
        }

        public HotelReservation theGuest_reservation(int reservationId)
        {
            HotelReservation hotelRes = db.HotelReservations
                                     .Include("Room.Hotel")
                                     .Include("Room")
                                     .Include("Room.Room")
                                     .Include("Guest")
                                     .Include("Room.Hotel.City")
                                     .Include("Room.Hotel.City.Country")
                                     .Include("Room.Hotel.Images")
                                     .Where(x => x.Id == reservationId
                                      && ((x.Check_In_Date > DateTime.Now) || (x.Check_Out_Date < DateTime.Now)))
                                     .SingleOrDefault();
            return hotelRes == null ? null : hotelRes;
        }

        public Hotel searchHotelRooms(int HotelId, string Type, int GuestsCount,
                        string DetailsTerm, float NightPriceGt, float NightPriceLs)
        {
            List<HotelRoom> hotelRooms = db.HotelRooms.Where(x => (x.Room.Type == Type || Type == "All")
                                                            && (x.Room.Cust_Count == GuestsCount || GuestsCount == null)
                                                            && (x.Room.Details_En.Contains(DetailsTerm)
                                                                || x.Room.Details_Ar.Contains(DetailsTerm)
                                                                || string.IsNullOrEmpty(DetailsTerm))

                                                            && x.Hotel.Id == HotelId)
                                                      .ToList();
            if (NightPriceGt != 0 && NightPriceLs != 0)
            {
                hotelRooms = hotelRooms.Where(x => (x.NightPrice > NightPriceGt
                                                   && (x.NightPrice <= NightPriceLs)
                                                   || NightPriceGt == 0 && NightPriceLs == 0))
                                       .ToList();
            }
            else
            {
                hotelRooms = hotelRooms.Where(x => (x.NightPrice > NightPriceGt
                                                   || (x.NightPrice <= NightPriceLs)
                                                   || NightPriceGt == 0 && NightPriceLs == 0))
                                       .ToList();
            }
            Hotel hotel = db.Hotels.Include("City")
                                              .Include("Images")
                                              .Include("HotelRooms")
                                              .Include("HotelRooms.Room")
                                              .SingleOrDefault(x => x.Id == HotelId);

            hotel.HotelRooms = hotelRooms;
            return hotel == null ? null : hotel;


        }

        //public HotelReservation reserveRoom(HotelReservation model, int RoomId)
        //{

        //}
    }


}