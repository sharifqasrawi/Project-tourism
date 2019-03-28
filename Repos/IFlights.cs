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
    internal interface IFlights
    {
        List<Flight> filtered_flights(int SourceCity, int DestinationCity, string DisplayDate);
        List<FlightReservation> customer_reservation_flights(string passengerId);
        FlightReservation theCustomer_reservation(int reservationId);


    }

    public class PostFlights : IFlights
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public List<Flight> filtered_flights(int SourceCity, int DestinationCity, string DisplayDate)
        {
            using (var context = new ApplicationDbContext())
            {
                List<Flight> fly = context.Flights
                           .Include("SourceCity")
                           .Include("DestinationCity")
                           .Where(x => (x.SourceCity.Id == SourceCity || SourceCity == null)
                                                          && (x.DestinationCity.Id == DestinationCity || DestinationCity == null)
                                                          && x.Date > DateTime.Now
                                                          && x.DisplayDate == DisplayDate)
                                                          .ToList();
                return fly == null ? null : fly;
            }
        }

        public List<FlightReservation> customer_reservation_flights(string Customer)
        {
            List<FlightReservation> flyRes = db.FlightReservations
                                  .Include("Flight")
                                  .Include("Flight.SourceCity")
                                  .Include("Flight.DestinationCity")
                                  .Include("Flight.DestinationCity.Images")
                                  .Include("Customer")
                                  .Where(x =>  x.Customer.Id == Customer
                                   && x.Flight.Date > DateTime.Now)
                                  .ToList();
            return flyRes == null ? null : flyRes;

        }

        public FlightReservation theCustomer_reservation(int reservationId)
        {
            FlightReservation myFlight = db.FlightReservations
                                  .Include("Flight")
                                  .Include("Flight.SourceCity")
                                  .Include("Flight.DestinationCity")
                                  .Include("Customer")
                                  .Where(x => x.ID == reservationId
                                   && x.Flight.Date > DateTime.Now)
                                  .SingleOrDefault();
            return myFlight == null ? null : myFlight;

        }
    }
}


