using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.Resources;
using TravelAndTourismWebsite.WebsiteLanguages;
using PagedList;
using PagedList.Mvc;
using Microsoft.AspNet.Identity;

namespace TravelAndTourismWebsite.Controllers
{
    public class OfferController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /*
         * View all offers
         */
        [AllowAnonymous]
        public ActionResult AllOffers(int? page)
        {
            List<Offer> offers = db.Offers.Include("FlightReservations")
                                          .Include("FlightReservations.Flight")
                                          .Include("FlightReservations.Flight.DestinationCity")
                                          .Include("FlightReservations.Flight.DestinationCity.Images")
                                          .Include("FlightReservations.Flight.DestinationCity.Country")
                                          .Include("HotelReservations")
                                          .Where(x => x.FlightReservations.FirstOrDefault().Flight.Date > DateTime.Now && x.Count > 0 && x.Status == "Available")
                                          .ToList();

            return View(offers.ToPagedList(page ?? 1, 10));
        }

        /*
         * Offer preview
         */
        [AllowAnonymous]
        public ActionResult OfferPreview(int Id)
        {
            Offer offer = db.Offers.Include("FlightReservations")
                                         .Include("FlightReservations.Flight")
                                         .Include("FlightReservations.Flight.SourceCity")
                                         .Include("FlightReservations.Flight.DestinationCity")
                                         .Include("FlightReservations.Flight.DestinationCity.Images")
                                         .Include("FlightReservations.Flight.DestinationCity.Country")
                                         .Include("FlightBackReservations")
                                         .Include("FlightBackReservations.Flight")
                                         .Include("FlightBackReservations.Flight.SourceCity")
                                         .Include("FlightBackReservations.Flight.DestinationCity")
                                         .Include("FlightBackReservations.Flight.DestinationCity.Images")
                                         .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                         .Include("HotelReservations")
                                         .Include("HotelReservations.Room")
                                         .Include("HotelReservations.Room.Room")
                                         .Include("HotelReservations.Room.Hotel")
                                         .Include("HotelReservations.Room.Hotel.Images")
                                         .SingleOrDefault(x => x.Id == Id);

            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                OfferReservation or = db.OfferReservations.SingleOrDefault(x => x.Customer.Id == user.Id && x.Offer.Id == offer.Id);
                if (or != null)
                {
                    ViewBag.Booked = true;
                }
            }

            return View(offer);
        }

        /*
         * Create new offer
         */

        [Authorize(Roles = "Admin,Offers Manager")]
        public ActionResult CreateOffer1()
        {
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");

                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");

                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");

                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
            }

            return View();
        }

        [Authorize(Roles = "Admin,Offers Manager")]
        public JsonResult Citylist(int id)
        {

            List<SelectListItem> li = new List<SelectListItem>();
            List<City> cities = db.Cities.Where(x => x.Country.Id == id).ToList();

            foreach (City c in cities)
            {
                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        li.Add(new SelectListItem() { Text = c.Name_En, Value = c.Id.ToString() });
                        break;
                    case "ar-SA":
                        li.Add(new SelectListItem() { Text = c.Name_Ar, Value = c.Id.ToString() });
                        break;

                    default:
                        li.Add(new SelectListItem() { Text = c.Name_En, Value = c.Id.ToString() });
                        break;
                }
            }

            return Json(li, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Offers Manager")]
        public ActionResult CreateOffer(int SourceCity, int DestinationCity)
        {
            List<Flight> flights = db.Flights.Include("SourceCity")
                                         .Include("DestinationCity")
                                         .Where(x => (x.SourceCity.Id == SourceCity || SourceCity == null)
                                            && (x.DestinationCity.Id == DestinationCity || DestinationCity == null)
                                            && x.Date > DateTime.Now)
                                         .ToList();

            List<Flight> flightsBack = db.Flights.Include("SourceCity")
                                       .Include("DestinationCity")
                                       .Where(x => (x.SourceCity.Id == DestinationCity)
                                          && (x.DestinationCity.Id == SourceCity)
                                          && x.Date > DateTime.Now)
                                       .ToList();

            List<HotelRoom> hotelRooms = db.HotelRooms.Include("Hotel")
                                           .Include("Room")
                                           .Include("Hotel.City")
                                           .Include("Hotel.City.Country")
                                           .Where(x => x.Hotel.City.Id == DestinationCity)
                                           .OrderBy(x => x.Hotel.Name_En)
                                           .ThenBy(x => x.Room.Type)
                                           .ToList();

            Session.Add("SourceCity", SourceCity);
            Session.Add("DestinationCity", DestinationCity);

            if (flights.Count == 0 || hotelRooms.Count == 0)
            {
                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");

                        ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        break;
                    case "ar-SA":
                        ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                        ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");

                        ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                        ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                        break;
                    default:
                        ViewBag.SourceCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        ViewBag.DestinationCountry = new SelectList(db.Countries.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");

                        ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                        break;
                }
                ViewBag.Message = Resource.NoFlightsOrHotels;
                ViewBag.Alert = "alert alert-danger";
                return View("CreateOffer1");
            }

            ViewBag.Flights = flights;
            ViewBag.FlightsBack = flightsBack;
            ViewBag.Hotels = hotelRooms;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Offers Manager")]
        public ActionResult CreateOffer(Offer model, int? SelectedFlight, int? SelectedFlightBack, string FlightClass, int? SelectedHotelRoom)
        {
            Flight flight = db.Flights.Find(SelectedFlight);
            Flight flightBack = db.Flights.Find(SelectedFlightBack);
            try
            {
                FlightReservation fr = new FlightReservation();

                FlightReservation fr2 = new FlightReservation();


                DateTime checkin = new DateTime(flight.Date.Year, flight.Date.Month, flight.Date.Day, 12, 0, 0, 0);
                DateTime checkout = new DateTime(flightBack.Date.Year, flightBack.Date.Month, flightBack.Date.Day, 12, 0, 0, 0);

                HotelReservation hr = new HotelReservation();


                List<FlightReservation> frs = new List<FlightReservation>();
                List<FlightReservation> frs2 = new List<FlightReservation>();
                List<HotelReservation> hrs = new List<HotelReservation>();

                for (int i = 0; i < model.Count; i++)
                {
                    fr = new FlightReservation()
                    {
                        DateTime = DateTime.Now,
                        DisplayDateTime = DateTime.Now.ToString(),
                        Flight = flight,
                        FlightClass = FlightClass,
                        Seats = model.FlightReservations[0].Seats,
                        ReservationPrice = 0,
                        Customer = null
                    };

                    switch (FlightClass)
                    {
                        case "FirstClass":
                            fr.ReservationPrice = fr.Seats * fr.Flight.FirstClassTicketPrice;
                            break;
                        case "Economy":
                            fr.ReservationPrice = fr.Seats * fr.Flight.EconomyTicketPrice;
                            break;
                        default:
                            fr.ReservationPrice = fr.Seats * fr.Flight.EconomyTicketPrice;
                            break;
                    }

                    flight.EcoSeatsCount -= model.FlightReservations[0].Seats;

                    fr2 = new FlightReservation()
                    {
                        DateTime = DateTime.Now,
                        DisplayDateTime = DateTime.Now.ToString(),
                        Flight = flightBack,
                        FlightClass = FlightClass,
                        Seats = model.FlightReservations[0].Seats,
                        ReservationPrice = 0,
                        Customer = null
                    };

                    switch (FlightClass)
                    {
                        case "FirstClass":
                            fr2.ReservationPrice = fr2.Seats * fr2.Flight.FirstClassTicketPrice;
                            break;
                        case "Economy":
                            fr2.ReservationPrice = fr2.Seats * fr2.Flight.EconomyTicketPrice;
                            break;
                        default:
                            fr2.ReservationPrice = fr2.Seats * fr2.Flight.EconomyTicketPrice;
                            break;
                    }
                    flightBack.EcoSeatsCount -= model.FlightReservations[0].Seats;

                    hr = new HotelReservation()
                    {
                        Check_In_Date = checkin,
                        DisplayCheck_In_Date = checkin.ToString(),
                        Check_Out_Date = checkout,
                        DisplayCheck_Out_Date = checkout.ToString(),
                        Room = db.HotelRooms.Find(SelectedHotelRoom),
                        ReservationCost = float.Parse(((checkout - checkin).TotalDays * db.HotelRooms.Find(SelectedHotelRoom).NightPrice).ToString()),
                        RoomsAvailable = db.HotelRooms.Find(SelectedHotelRoom).Count - 1,
                        Guest = null
                    };


                    frs.Add(fr);
                    frs2.Add(fr2);
                    hrs.Add(hr);
                }

                float flightPrice = 0;
                float hotelPrice = 0;
                flightPrice = float.Parse((fr.ReservationPrice + fr2.ReservationPrice).ToString());
                hotelPrice = hr.ReservationCost;

                Offer offer = new Offer()
                {
                    CustomersCount = model.CustomersCount,
                    Price = flightPrice + hotelPrice,
                    Discount = model.Discount,
                    NewPrice = 0,
                    Count = model.Count,
                    Details_En = model.Details_En,
                    Details_Ar = model.Details_Ar,
                    Duration = int.Parse((flightBack.Date - flight.Date).TotalDays.ToString()),
                    Status = "Available"
                };

                db.Offers.Add(offer);
                offer.FlightReservations = frs;
                offer.FlightBackReservations = frs2;
                offer.HotelReservations = hrs;

                offer.NewPrice = offer.Price - (offer.Price * offer.Discount);
                db.SaveChanges();

                Session.Remove("SourceCity");
                Session.Remove("DestinationCity");

                return RedirectToAction("Index", "Admin");
            }
            catch
            {

            }

            int SourceCity = int.Parse(Session["SourceCity"].ToString());
            int DestinationCity = int.Parse(Session["DestinationCity"].ToString());

            List<Flight> flights = db.Flights.Include("SourceCity")
                                      .Include("DestinationCity")
                                      .Where(x => x.SourceCity.Id == SourceCity
                                         && x.DestinationCity.Id == DestinationCity
                                         && x.Date > DateTime.Now)
                                      .ToList();

            List<Flight> flightsBack = db.Flights.Include("SourceCity")
                                       .Include("DestinationCity")
                                       .Where(x => x.SourceCity.Id == DestinationCity
                                          && x.DestinationCity.Id == SourceCity
                                          && x.Date > DateTime.Now)
                                       .ToList();

            List<HotelRoom> hotelRooms = db.HotelRooms.Include("Hotel")
                                           .Include("Room")
                                           .Include("Hotel.City")
                                           .Include("Hotel.City.Country")
                                           .Where(x => x.Hotel.City.Id == DestinationCity)
                                           .OrderBy(x => x.Hotel.Name_En)
                                           .ThenBy(x => x.Room.Type)
                                           .ToList();


            ViewBag.Flights = flights;
            ViewBag.FlightsBack = flightsBack;
            ViewBag.Hotels = hotelRooms;

            return View(model);
        }

        /*
         * Book offer
         */
        [Authorize(Roles = "User")]
        public ActionResult Book(int Id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            Offer offer = db.Offers.Include("FlightReservations")
                                         .Include("FlightReservations.Flight")
                                         .Include("FlightReservations.Flight.SourceCity")
                                         .Include("FlightReservations.Flight.DestinationCity")
                                         .Include("FlightReservations.Flight.DestinationCity.Images")
                                         .Include("FlightReservations.Flight.DestinationCity.Country")
                                         .Include("FlightBackReservations")
                                         .Include("FlightBackReservations.Flight")
                                         .Include("FlightBackReservations.Flight.SourceCity")
                                         .Include("FlightBackReservations.Flight.DestinationCity")
                                         .Include("FlightBackReservations.Flight.DestinationCity.Images")
                                         .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                         .Include("HotelReservations")
                                         .Include("HotelReservations.Room")
                                         .Include("HotelReservations.Room.Room")
                                         .Include("HotelReservations.Room.Hotel")
                                         .Include("HotelReservations.Room.Hotel.Images")
                                         .SingleOrDefault(x => x.Id == Id);


            if (offer.NewPrice > user.Credit)
            {
                ViewBag.Alert = "alert alert-danger";
                ViewBag.Message = Resource.NotEnoughCredit;

                return View("OfferPreview", offer);
            }

            OfferReservation or = new OfferReservation()
            {
                Customer = user,
                Offer = offer,
                DateTime = DateTime.Now,
            };

            db.OfferReservations.Add(or);

            HotelReservation hr = or.Offer.HotelReservations.Where(x => x.IsBooked == false).FirstOrDefault();
            hr.Guest = user;
            hr.IsBooked = true;

            FlightReservation fr = or.Offer.FlightReservations.Where(x => x.IsBooked == false).FirstOrDefault();
            fr.Customer = user;
            fr.IsBooked = true;

            FlightReservation fbr = or.Offer.FlightBackReservations.Where(x => x.IsBooked == false).FirstOrDefault();
            fbr.Customer = user;
            fbr.IsBooked = true;

            user.Credit -= offer.NewPrice;
            offer.Count--;

            db.SaveChanges();

            or = db.OfferReservations.SingleOrDefault(x => x.Customer.Id == user.Id && x.Offer.Id == offer.Id);
            if (or != null)
            {
                ViewBag.Booked = true;
            }

            ViewBag.Alert = "alert alert-success";
            ViewBag.Message = Resource.OfferBooked;
            return RedirectToAction("Reservation", new { Id = or.Id });
        }
        /*
         * Offer reservation 
         */
        [Authorize(Roles = "User")]
        public ActionResult Reservation(int Id)
        {
            OfferReservation offer = db.OfferReservations.Include("Offer.FlightReservations")
                                                       .Include("Offer.FlightReservations.Flight")
                                                       .Include("Offer.FlightReservations.Flight.SourceCity")
                                                       .Include("Offer.FlightReservations.Flight.SourceCity.Country")
                                                       .Include("Offer.FlightReservations.Flight.DestinationCity")
                                                       .Include("Offer.FlightBackReservations")
                                                       .Include("Offer.FlightBackReservations.Flight")
                                                       .Include("Offer.FlightBackReservations.Flight.SourceCity")
                                                       .Include("Offer.FlightBackReservations.Flight.DestinationCity")
                                                       .Include("Offer.HotelReservations")
                                                       .Include("Offer.HotelReservations.Room")
                                                       .Include("Offer.HotelReservations.Room.Room")
                                                       .Include("Offer.HotelReservations.Room.Hotel")
                                                       .Include("Customer")
                                                       .SingleOrDefault(x => x.Id == Id);

            return View(offer);
        }

        [Authorize(Roles = "User")]
        public ActionResult MyOffers(int? page)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            List<OfferReservation> ors = db.OfferReservations.Include("Offer")
                                                             .Include("Offer.FlightReservations.Flight")
                                                             .Include("Offer.FlightReservations.Flight.DestinationCity.Country")
                                                             .Include("Offer.FlightReservations.Flight.DestinationCity")
                                                             .Where(x => x.Customer.Id == user.Id)
                                                             .ToList();

            return View(ors.ToPagedList(page ?? 1, 20));
        }

        [Authorize(Roles = "Admin,Offers Manager")]
        public ActionResult OffersManagement(int? page)
        {
            List<Offer> offers = db.Offers.Include("FlightReservations")
                                        .Include("FlightReservations.Flight")
                                        .Include("FlightReservations.Flight.DestinationCity")
                                        .Include("FlightReservations.Flight.SourceCity")
                                        .Include("FlightReservations.Flight.DestinationCity.Images")
                                        .Include("FlightReservations.Flight.DestinationCity.Country")
                                        .Include("HotelReservations")
                                        .ToList();

            return View(offers.ToPagedList(page ?? 1, 10));
        }

        /*
         * Close offer
         */

        [Authorize(Roles = "Admin,Offers Manager")]
        public ActionResult CloseOffer(int id)
        {
            Offer offer = db.Offers.Include("FlightReservations")
                                       .Include("FlightReservations.Flight")
                                       .Include("FlightReservations.Flight.SourceCity")
                                       .Include("FlightReservations.Flight.DestinationCity")
                                       .Include("FlightReservations.Flight.DestinationCity.Images")
                                       .Include("FlightReservations.Flight.DestinationCity.Country")
                                       .Include("FlightBackReservations")
                                       .Include("FlightBackReservations.Flight")
                                       .Include("FlightBackReservations.Flight.SourceCity")
                                       .Include("FlightBackReservations.Flight.DestinationCity")
                                       .Include("FlightBackReservations.Flight.DestinationCity.Images")
                                       .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                       .Include("HotelReservations")
                                       .Include("HotelReservations.Room")
                                       .Include("HotelReservations.Room.Room")
                                       .Include("HotelReservations.Room.Hotel")
                                       .Include("HotelReservations.Room.Hotel.Images")
                                       .SingleOrDefault(x => x.Id == id);

            offer.Count -= offer.Count;

            List<FlightReservation> frs = offer.FlightReservations.Where(x => x.IsBooked == false).ToList();
            List<FlightReservation> fbrs = offer.FlightBackReservations.Where(x => x.IsBooked == false).ToList();
            List<HotelReservation> hrs = offer.HotelReservations.Where(x => x.IsBooked == false).ToList();

            for (int i = 0; i < frs.Count; i++)
            {
                db.FlightReservations.Remove(frs[i]);
                db.FlightReservations.Remove(fbrs[i]);
                db.HotelReservations.Remove(hrs[i]);
            }

            offer.Status = "Closed";

            db.SaveChanges();
            return RedirectToAction("OffersManagement", "Offer");
        }

        /*
         * Cancel offer reservation 
         */

        [Authorize(Roles = "User")]
        public ActionResult CancelReservation(int Id)
        {
            OfferReservation or = db.OfferReservations.Include("Offer")
                                                     .Include("Offer.FlightReservations")
                                                     .Include("Offer.FlightReservations.Customer")
                                                     .Include("Offer.FlightBackReservations")
                                                     .Include("Offer.FlightBackReservations.Customer")
                                                     .Include("Offer.HotelReservations")
                                                     .Include("Offer.HotelReservations.Guest")
                                                     .SingleOrDefault(x => x.Id == Id);

            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            user.Credit += float.Parse(or.Offer.NewPrice.ToString());
            or.Offer.Count += 1;

            FlightReservation fr = or.Offer.FlightReservations.FirstOrDefault(x => x.Customer.Id == user.Id);
            fr.Customer = null;
            fr.IsBooked = false;

            FlightReservation fbr = or.Offer.FlightBackReservations.FirstOrDefault(x => x.Customer.Id == user.Id);
            fbr.Customer = null;
            fbr.IsBooked = false;

            HotelReservation hr = or.Offer.HotelReservations.FirstOrDefault(x => x.Guest.Id == user.Id);
            hr.Guest = null;
            hr.IsBooked = false;

            db.OfferReservations.Remove(or);
            db.SaveChanges();

            return RedirectToAction("MyOffers", "Offer");
        }

        /*
         * View Offer for admin
         */
        public ActionResult ViewOffer(int Id)
        {
            Offer offer = db.Offers.Include("FlightReservations")
                                              .Include("FlightReservations.Flight")
                                              .Include("FlightReservations.Flight.SourceCity")
                                              .Include("FlightReservations.Flight.DestinationCity")
                                              .Include("FlightReservations.Flight.DestinationCity.Images")
                                              .Include("FlightReservations.Flight.DestinationCity.Country")
                                              .Include("FlightBackReservations")
                                              .Include("FlightBackReservations.Flight")
                                              .Include("FlightBackReservations.Flight.SourceCity")
                                              .Include("FlightBackReservations.Flight.DestinationCity")
                                              .Include("FlightBackReservations.Flight.DestinationCity.Images")
                                              .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                              .Include("HotelReservations")
                                              .Include("HotelReservations.Room")
                                              .Include("HotelReservations.Room.Room")
                                              .Include("HotelReservations.Room.Hotel")
                                              .Include("HotelReservations.Room.Hotel.Images")
                                              .SingleOrDefault(x => x.Id == Id);

            return View(offer);

        }
    }
}
