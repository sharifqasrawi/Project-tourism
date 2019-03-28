using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.WebsiteLanguages;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Controllers
{
    public class FlightController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /*
         * All flights
         */
        [AllowAnonymous]
        public ActionResult Flights(int? page, int? SourceCity, int? DestinationCity,DateTime? Date)
        {
            IPagedList<Flight> flights = db.Flights.Include("SourceCity")
                                                 .Include("DestinationCity")
                                                 .Where(x => (x.SourceCity.Id == SourceCity || SourceCity == null)
                                                          && (x.DestinationCity.Id == DestinationCity || DestinationCity == null)
                                                          && (x.Date > DateTime.Now)
                                                          && (x.Date == Date || Date == null))
                                                 .ToList()
                                                 .ToPagedList(page ?? 1, 15);

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "ID", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "ID", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "ID", "Name_Ar");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "ID", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "ID", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "ID", "Name_En");
                    break;
            }
            return View(flights);
        }

        /*
         * Book seats in flight
         */

        [Authorize(Roles="User")]
        public ActionResult BookFlight(int id)
        {
            ViewBag.FlightID = id;
            ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult BookFlight(FlightReservation model, int FlightID, string FlightClass)
        {
            try
            {
                Flight f = db.Flights.Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);
                if(model.Seats == 0)
                {
                    ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);

                    ModelState.AddModelError("","");
                    return View(model);
                }

                switch (FlightClass)
                {
                    case "FirstClass":
                        if (model.Seats > f.FrstSeatsCount)
                        {
                            ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);
                            ViewBag.Message = Resource.NotEnoughSeats;
                            ViewBag.Alert = "alert alert-danger";
                            return View(model);
                        }
                        break;
                    case "Economy":
                        if (model.Seats > f.EcoSeatsCount)
                        {
                            ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);
                            ViewBag.Message = Resource.NotEnoughSeats;
                            ViewBag.Alert = "alert alert-danger";
                            return View(model);
                        }
                        break;
                    default:
                        if (model.Seats > f.EcoSeatsCount)
                        {
                            ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);
                            ViewBag.Message = Resource.NotEnoughSeats;
                            ViewBag.Alert = "alert alert-danger";
                            return View(model);
                        }
                        break;
                }

              

                FlightReservation fr = new FlightReservation()
                {
                    Customer = db.Users.Find(User.Identity.GetUserId()),
                    Flight = f,
                    FlightClass = model.FlightClass,
                    Seats = model.Seats,
                    DateTime = DateTime.Now,
                    DisplayDateTime = DateTime.Now.ToString(),
                    IsBooked = true
                };
                switch(FlightClass)
                {
                    case "FirstClass":
                        fr.ReservationPrice = fr.Seats *  fr.Flight.FirstClassTicketPrice;
                        break;
                    case "Economy":
                        fr.ReservationPrice = fr.Seats * fr.Flight.EconomyTicketPrice;
                        break;
                    default:
                        fr.ReservationPrice = fr.Seats * fr.Flight.EconomyTicketPrice;
                        break;
                }

                if (fr.ReservationPrice > db.Users.Find(User.Identity.GetUserId()).Credit)
                {
                    ViewBag.FlightID = FlightID;
                    ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);

                    ViewBag.Message = Resource.NotEnoughCredit;
                    ViewBag.Alert = "alert alert-danger";
                    return View(model);
                }

                db.FlightReservations.Add(fr);

                switch (FlightClass)
                {
                    case "FirstClass":
                        f.FrstSeatsCount -= model.Seats;
                        break;
                    case "Economy":
                        f.EcoSeatsCount -= model.Seats;
                        break;
                    default:
                        f.EcoSeatsCount -= model.Seats;
                        break;
                }

                
                db.Users.Find(User.Identity.GetUserId()).Credit -= float.Parse(fr.ReservationPrice.ToString());

                db.SaveChanges();

                return RedirectToAction("SuggestedHotels", "Hotel", new { CityId = f.DestinationCity.Id , ResId = fr.ID});
            }
            catch
            {

            }
            ViewBag.FlightID = FlightID;
            ViewBag.Flight = db.Flights.Include("SourceCity").Include("DestinationCity").SingleOrDefault(x => x.ID == FlightID);

            return View(model);
        }

        /*
         * View user's flights
         */
        [Authorize(Roles = "User")]
        public ActionResult MyFlights()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            List<FlightReservation> flights = db.FlightReservations.Include("Flight")
                                                                   .Include("Flight.SourceCity")
                                                                   .Include("Flight.DestinationCity")
                                                                   .Where(x => x.Customer.Id == user.Id)
                                                                   .ToList();

            return View(flights);
        }

        /*
         * Flight Ticket 
         */

        [Authorize(Roles = "User")]
        public ActionResult Ticket(int Id)
        {
            FlightReservation fr = db.FlightReservations.Include("Flight")
                                                        .Include("Flight.SourceCity")
                                                        .Include("Flight.DestinationCity")
                                                        .Include("Customer")
                                                        .SingleOrDefault(x => x.ID == Id);
            return View(fr);
        }


        /*
         * Cancel Flight reservation by user
         */
        [Authorize(Roles = "User")]
        public ActionResult CancelReservation(int Id)
        {
            FlightReservation fr = db.FlightReservations.Include("Flight").SingleOrDefault(x => x.ID == Id);
            Flight f = db.Flights.Find(fr.Flight.ID);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            user.Credit += float.Parse(fr.ReservationPrice.ToString());

            switch(fr.FlightClass)
            {
                case "FirstClass":
                    f.FrstSeatsCount += fr.Seats;
                    break;
                case "EconomyClass":
                    f.EcoSeatsCount += fr.Seats;
                    break;
                default:
                    f.EcoSeatsCount += fr.Seats;
                    break;

            }
           

            db.FlightReservations.Remove(fr);
            db.SaveChanges();

            return RedirectToAction("MyFlights", "Flight");
        }
	}
}