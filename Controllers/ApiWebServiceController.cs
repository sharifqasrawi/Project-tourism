using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Data.Entity;
using System.Threading.Tasks;
using System;
using Microsoft.AspNet.Identity;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.Resources;
using TravelAndTourismWebsite.Repos;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Security.Cryptography;
using System.Text;

namespace TravelAndTourismWebsite.Controllers
{
    public class ApiWebServiceController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Country API Controller
        /// </summary>
        [RoutePrefix("api/countries")]
        public class ApiCountryController : ApiController
        {

            [HttpGet]
            [Route("")]
            public IHttpActionResult GetAllCountries()
            {
                using (var context = new ApplicationDbContext())
                {

                    List<Country> Countries = context.Countries
                                                   .Include("Cities")
                                                   .Include("Cities.Images")
                                                   .Where(x =>
                                                       x.Id == x.Id && x.Visible==true)
                                                       .ToList();


                    return Ok(new { contries = Countries });
                }
            }// end GetAllCountries

            [HttpGet]
            [Route("{id}")]
            public IHttpActionResult GetCountry(int id)
            {
                using (var context = new ApplicationDbContext())
                {
                    Country Country = context.Countries
                                                  .Include("Cities")
                                                  .Include("Cities.Images")
                                                  .Where(x =>
                                                      x.Id == id && x.Visible==true)
                                                      .SingleOrDefault();

                    return Ok(new { country = Country });

                }
            }// end GetCountry by Id

        }// end country class


        /// <summary>
        /// Cities API Controller
        /// </summary>
        [RoutePrefix("api/cities")]
        public class ApiCitiesController : ApiController
        {
            [HttpGet]
            [Route("")]
            public IHttpActionResult GetAllCities()
            {

                using (var context = new ApplicationDbContext())
                {
                    List<City> Cities = context.Cities
                                                  .Include("Country")
                                                  .Include("Images")
                                                  .Where(x =>
                                                      x.Id == x.Id)
                                                      .ToList();

                    return Ok(new { Cities = Cities });

                }
            }// end getAllCities


            [HttpGet]
            [Route("{id}")]
            public IHttpActionResult GetCity(int id)
            {
                using (var context = new ApplicationDbContext())
                {
                    City City = context.Cities
                                                 .Include("Country")
                                                 .Include("Images")
                                                 .Where(x =>
                                                     x.Id == id)
                                                     .SingleOrDefault();
                    return Ok(new { City = City });

                }
            }// end GetCity                

        }//end Cities class


        /// <summary>
        /// Hotel API Controller
        /// </summary>
        [RoutePrefix("api/Hotel")]
        public class ApiHotelController : ApiController
        {
            private readonly IHotels _hotels;
            public ApiHotelController()
            {
                _hotels = new PostHotels();
            }

            [HttpGet]
            [Route("")]
            public IHttpActionResult GetAllHotels()
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Hotel> hotels = context.Hotels
                                                .Include("City")
                                                .Include("City.Country")
                                                .Include("Images")
                                                .Include("HotelRooms")
                                                .Include("HotelRooms.Room")
                                                .Include("HotelRates")
                                                .Where(x =>
                                                    ((x.City.Id == x.City.Id)) &&
                                                    (x.City.Country.Id == x.City.Country.Id))
                                                    .ToList();


                    return Ok(new { Hotels = hotels });
                }// using
            }// end all hotel get

            [HttpGet]
            [Route("{hotelId}")]
            public IHttpActionResult GetHotelByID(int? hotelId)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                //using (var context = new ApplicationDbContext())
                //{
                Hotel hotel = db.Hotels
                                            .Include("City")
                                            .Include("City.Images")
                                            .Include("City.Country")
                                            .Include("Images")
                                            .Include("HotelRooms")
                                            .Include("HotelRooms.Room")
                                            .Include("HotelRates")
                                            .Include("HotelRates.User")
                                            .SingleOrDefault(x => x.Id == hotelId);

                return Ok(new { Hotel = hotel });
                //  }//end  using
            }// end all hotel get


            // by country and city And NAME
            [HttpGet]
            [Route("searchHotelName/{countryId}/{cityId}/{hotelName}")] // localhost:11321/api/hotels/1/11
            public IHttpActionResult GetNameHotels(int countryId, int cityId, string hotelName)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Hotel> hotels = context.Hotels
                                                .Include("City")
                                                .Include("City.Country")
                                                .Include("Images")
                                                .Include("HotelRooms")
                                                .Include("HotelRooms.Room")
                                                .Include("HotelRates")
                                                .Where(x =>
                                                    (x.City.Id == cityId || cityId == null) &&
                                                    (x.City.Country.Id == countryId || countryId == null) &&
                                                    (x.Name_En.ToLower().Contains(hotelName.ToLower()) || string.IsNullOrEmpty(hotelName))
                                                       || (x.Name_Ar.Contains(hotelName) || string.IsNullOrEmpty(hotelName)))
                                                    .ToList();


                    return Ok(new { Hotels = hotels });
                }// using
            }


            // by country and city And NAME
            [HttpPost]
            [Route("SearchHotelByName")] // localhost:11321/api/hotels/1/11/hotelName
            public IHttpActionResult GetNameHotels([FromBody] ApiModelsView.HotelByNameModelView model)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Hotel> hotels = context.Hotels
                                                .Include("City")
                                                .Include("City.Country")
                                                .Include("Images")
                                                .Include("HotelRooms")
                                                .Include("HotelRooms.Room")
                                                .Include("HotelRates")
                                                .Where(x =>
                                                    (x.City.Id == model.City || model.City == null) &&
                                                    (x.City.Country.Id == model.Country || model.Country == null) &&
                                                    (x.Name_En.ToLower().Contains(model.HotelName.ToLower()) || string.IsNullOrEmpty(model.HotelName))
                                                       || (x.Name_Ar.Contains(model.HotelName) || string.IsNullOrEmpty(model.HotelName)))
                                                    .ToList();


                    return Ok(new { Hotels = hotels });
                }// using
            }


            // by country and city
            [HttpGet]
            [Route("{countryId}/{cityId}")] // localhost:11321/api/hotels/1/11
            public IHttpActionResult GetCityHotels(int? CityId, int? CountryId)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Hotel> hotels = context.Hotels
                                                .Include("City")
                                                .Include("City.Country")
                                                .Include("Images")
                                                .Include("HotelRooms")
                                                .Include("HotelRooms.Room")
                                                .Include("HotelRates")
                                                .Where(x =>
                                                    ((x.City.Id == CityId) || CityId == null) &&
                                                    (x.City.Country.Id == CountryId || CountryId == null))
                                                    .ToList();


                    return Ok(new { Hotels = hotels });
                }// using
            }// end get hotel by country and city


            // by country and city  AND  Stars
            [HttpGet]
            [Route("{countryId}/{cityId}/{starsNum}")] // localhost:11321/api/hotels/1/11
            public IHttpActionResult GetStarsHotels(int? CityId, int? CountryId, int? starsNum)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Hotel> hotels = context.Hotels
                                                .Include("City")
                                                .Include("City.Country")
                                                .Include("Images")
                                                .Include("HotelRooms")
                                                .Include("HotelRooms.Room")
                                                .Include("HotelRates")
                                                .Where(x =>
                                                    ((x.City.Id == CityId) || CityId == null) &&
                                                    (x.City.Country.Id == CountryId || CountryId == null) &&
                                                    (x.Stars == starsNum || starsNum == null))
                                                    .ToList();


                    return Ok(new { Hotels = hotels });
                }// using
            }// end get hotel and stars


            [HttpPost]
            [Route("Reservations")] // My List of Reservations: guestId
            public IHttpActionResult PostAllGuestReservation([FromBody] ApiModelsView.HotelReservationModelView reservation)
            {
                if (reservation == null)
                    return BadRequest("Please Provide Prameter");

                // var res = _flights.customer_reservation_flights(reservation.Customer);
                var res = _hotels.Guest_reservation_hotels(reservation.Guest);
                if (res == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { HotelReservations = res.ToList() });
            }// end PostAllCousReservation

            [HttpPost]
            [Route("MyReservation")] // My single reservation: reservationId
            public IHttpActionResult PostTheReservation([FromBody] HotelReservation reservation)
            {
                if (reservation == null)
                    return BadRequest("Please Provide Prameter");

                var res = _hotels.theGuest_reservation(reservation.Id);
                if (res == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { HotelReservation = res });
            }// end PostAllCousReservation

            [HttpPost]
            [Route("Room")] // Search Room 
            public IHttpActionResult SearchRoom([FromBody] ApiModelsView.HotelRoomsModelView model)
            {
                if (model == null)
                    return BadRequest("Please Provide Prameter");

                var res = _hotels.searchHotelRooms(model.HotelId, model.Type, model.GuestsCount,
                    model.DetailsTerm, model.NightPriceGt, model.NightPriceLs);
                if (res == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { Room = res });
            }// end PostAllCousReservation

            [HttpPost]
            [Route("ReserveHotel")]// reserv
            public IHttpActionResult ReserveRoom([FromBody] ApiModelsView.HotelReservationModelView model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                ApplicationDbContext db = new ApplicationDbContext();
                HotelRoom room = db.HotelRooms.Include("Hotel")
                                             .Include("Room")
                                             .Include("Hotel.City")
                                             .Include("Hotel.City.Country")
                                             .SingleOrDefault(x => x.Id == model.RoomId);


                try
                {
                    int AvailableRoomsCount = 0;

                    DateTime checkin = new DateTime(model.Check_In_Date.Year, model.Check_In_Date.Month, model.Check_In_Date.Day, 12, 0, 0, 0);
                    DateTime checkout = new DateTime(model.Check_Out_Date.Year, model.Check_Out_Date.Month, model.Check_Out_Date.Day, 12, 0, 0, 0);

                    try
                    {
                        DateTime MaxCheckInDate = db.HotelReservations.Where(x => x.Room.Id == model.RoomId).Max(x => x.Check_In_Date);

                        DateTime MinCheckOutDate = db.HotelReservations.Where(x => x.Room.Id == model.RoomId).Min(x => x.Check_Out_Date);

                        AvailableRoomsCount = db.HotelReservations.Where(x => x.Room.Id == model.RoomId && checkin >= MaxCheckInDate && checkin <= MinCheckOutDate)
                                                                     .Min(x => x.RoomsAvailable);

                    }
                    catch
                    {
                        AvailableRoomsCount = room.Count;
                    }

                    if (AvailableRoomsCount == 0)
                    {

                        //  return Created(new Uri(Url.Link("Default", new { id = savedRecord.followUp_Id })), ConvertTableEntryToViewModel(savedRecord));
                        //  return Ok(new { Room = res });
                        return BadRequest("Roome is not available");

                    }
                    string userId = model.Guest;

                    HotelReservation res = new HotelReservation()
                    {
                        Room = room,
                        Check_In_Date = checkin,
                        Check_Out_Date = checkout,
                        DisplayCheck_In_Date = checkin.ToString(),
                        DisplayCheck_Out_Date = checkout.ToString(),
                        Guest = db.Users.Find(userId),
                        ReservationCost = float.Parse(((checkout - checkin).TotalDays * room.NightPrice).ToString()),
                        RoomsAvailable = AvailableRoomsCount - 1,
                        IsBooked = true,

                    };

                    if (res.ReservationCost > db.Users.Find(userId).Credit)
                    {
                        return BadRequest("Your must recharge your cridet to compleate the reservation");

                    }

                    db.HotelReservations.Add(res);
                    db.Users.Find(userId).Credit -= res.ReservationCost;


                    db.SaveChanges();
                    return Ok(new { Confermation = res });

                }
                catch
                { }
                return BadRequest();
            }// end reserve room


            [HttpPost]
            [Route("HotelRate")]
            public IHttpActionResult HotelRate([FromBody] ApiModelsView.HotelRateModelView model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                Hotel hotel = db.Hotels.Include("City")
                                 .Include("Images")
                                 .Include("HotelRooms")
                                 .Include("HotelRooms.Room")
                                 .Include("HotelRates")
                                 .SingleOrDefault(x => x.Id == model.HotelId);

                string userId = model.User;

                HotelRate hr = db.HotelRates.SingleOrDefault(x => x.Hotel.Id == hotel.Id && x.User.Id == userId);

                if (hr == null)
                {
                    hr = new HotelRate()
                    {
                        Hotel = hotel,
                        User = db.Users.Find(userId),
                        Rate = model.Rate
                    };
                    db.HotelRates.Add(hr);
                }
                else
                {
                    hr.Rate = model.Rate;
                }
                db.SaveChanges();
                return Ok(new { YourRate = hr });

            }


            [HttpPost]
            [Route("CancelHotelReservation")]
            public IHttpActionResult CancelReserveHotel([FromBody] ApiModelsView.HotelReservationModelView model)
            {
                try 
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    var user1 = model.Guest;
                    ApplicationUser user = db.Users.Find(user1);

                    HotelReservation hr = db.HotelReservations.Find(model.Id);

                    user.Credit += float.Parse(hr.ReservationCost.ToString());


                    db.HotelReservations.Remove(hr);
                    db.SaveChanges();

                    return Ok(new { Message = "Hotel Reservation Canceled.." });
                }
                catch 
                {
                }

                return BadRequest();
              

            }
        }// end class hotels


        /// <summary>
        /// Flight API Controller
        /// </summary>
        [RoutePrefix("api/flights")]
        public class ApiFlightsController : ApiController
        {
            private readonly IFlights _flights;
            public ApiFlightsController()
            {
                _flights = new PostFlights();
            }

            [HttpGet] // Flights shadule
            [Route("{SourceCity}/{DestinationCity}")]
            public IHttpActionResult GetAllFligts(int? SourceCity, int? DestinationCity)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Flight> flights = context.Flights
                        .Include("SourceCity")
                        .Include("DestinationCity")
                        .Where(x => (x.SourceCity.Id == SourceCity || SourceCity == null)
                                                       && (x.DestinationCity.Id == DestinationCity || DestinationCity == null)
                                                       && x.Date > DateTime.Now)
                                                       .ToList();
                    return Ok(new { Flights = flights });

                }
            }

            [HttpPost]
            [Route("")] //ALL FLIGHT: sourceCity/DestanationCity/Date
            public IHttpActionResult PostAllFligts([FromBody] ApiModelsView.FlightModelView flights)
            {
                if (flights == null)
                    return BadRequest("Please Provide Prameter");

                var flight = _flights.filtered_flights(flights.SourceCity, flights.DestinationCity, flights.DisplayDate);

                if (flight == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { Flight = flight.ToList() });
            }

            [HttpPost]
            [Route("Reservations")] // My List of Reservations: passengerId
            public IHttpActionResult PostAllCousReservation([FromBody] ApiModelsView.FlightReservationsViewModel reservation)
            {
                if (reservation == null)
                    return BadRequest("Please Provide Prameter");

                string cc = reservation.Customer;
                var res = _flights.customer_reservation_flights(reservation.Customer);

                if (res == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { FlightReservations = res });
            }// end PostAllCousReservation

            [HttpPost]
            [Route("MyReservation")] // My single reservation: reservationId
            public IHttpActionResult PostTheReservation([FromBody] ApiModelsView.FlightReservationsViewModel reservation)
            {
                if (reservation == null)
                    return BadRequest("Please Provide Prameter");

                var res = _flights.theCustomer_reservation(reservation.ID);

                if (res == null)
                {
                    return Ok(new { message = "No Flights available" });
                }
                return Ok(new { FlightReservation = res });
            }// end PostAllCousReservation

            [HttpPost]
            [Route("ReserveFlight")]
            public IHttpActionResult ReservaFlight([FromBody] ApiModelsView.FlightReservationsViewModel model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                try
                {
                    Flight f = db.Flights.Include("DestinationCity")
                                         .Include("SourceCity")
                                         .Include("DestinationCity.Images")
                                         .SingleOrDefault(x => x.ID == model.FlightId);
                    if (model.Seats == 0)
                    {
                        return BadRequest("Please provide seats number..");
                    }

                    switch (model.FlightClass)
                    {
                        case "FirstClass":
                            if (model.Seats > f.FrstSeatsCount)
                            {
                                return BadRequest("No Enough Seat in First class...");

                            }
                            break;
                        case "Economy":
                            if (model.Seats > f.EcoSeatsCount)
                            {
                                return BadRequest("No Enough Seat in Economy class...");

                            }
                            break;
                        default:
                            if (model.Seats > f.EcoSeatsCount)
                            {
                                return BadRequest("No Enough Seat ...");

                            }
                            break;
                    }

                    string userId = model.Customer;

                    FlightReservation fr = new FlightReservation()
                    {
                        Customer = db.Users.Find(userId),
                        Flight = f,
                        FlightClass = model.FlightClass,
                        Seats = model.Seats,
                        DateTime = DateTime.Now,
                        DisplayDateTime = DateTime.Now.ToString(),
                        IsBooked = true
                    };


                    switch (model.FlightClass)
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

                    if (fr.ReservationPrice > db.Users.Find(userId).Credit)
                    {
                        return BadRequest("Not Enough Credit");

                    }

                    db.FlightReservations.Add(fr);

                    switch (model.FlightClass)
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

                    ApplicationUser ss = db.Users.Find(userId);
                    float cridetUser = ss.Credit;
                    ss.Credit -= float.Parse(fr.ReservationPrice.ToString());
                    //  db.Users.Find(ss.Credit -= float.Parse(fr.ReservationPrice.ToString()));

                    db.SaveChanges();
                    return Ok(new { FlightConfermation = fr });

                }
                catch
                {

                }
                return BadRequest(ModelState);

            } // End Reserve Flight

            [HttpPost]
            [Route("CancelFlightReservation")]
            public IHttpActionResult CancelReserveFlight([FromBody] ApiModelsView.FlightReservationsViewModel model)
            {
                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    FlightReservation fr = db.FlightReservations.Include("Flight").SingleOrDefault(x => x.ID == model.ID);
                    Flight f = db.Flights.Find(fr.Flight.ID);



                    var user1 = model.Customer;
                    ApplicationUser user = db.Users.Find(user1);

                    user.Credit += float.Parse(fr.ReservationPrice.ToString());

                    switch (fr.FlightClass)
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

                    return Ok(new { Message = "The Flight Reservation Canceled" });
                }
                catch { }

                return BadRequest();
            }


        }// end flight api controller

        /// <summary>
        /// Offers API Controller
        /// </summary>
        [RoutePrefix("api/offers")]
        public class ApiOffersController : ApiController
        {
            [HttpGet]
            [Route("")]
            public IHttpActionResult GetAllOffers()
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Offer> offers = context.Offers.Include("FlightReservations")
                                     .Include("FlightReservations.Flight")
                                     .Include("flightBackReservations.Flight")
                                     .Include("FlightReservations.Flight.DestinationCity")
                                     .Include("FlightReservations.Flight.SourceCity")
                                     .Include("FlightBackReservations.Flight.DestinationCity")
                                     .Include("FlightBackReservations.Flight.SourceCity")
                                     .Include("FlightReservations.Flight.DestinationCity.Country")
                                     .Include("FlightReservations.Flight.SourceCity.Country")
                                     .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                     .Include("FlightBackReservations.Flight.SourceCity.Country")
                                     .Include("FlightReservations.Flight.DestinationCity.Images")
                                     .Include("HotelReservations")
                                     .Include("HotelReservations.Room")
                                     .Include("HotelReservations.Room.Room")
                                     .Include("HotelReservations.Room.Hotel.Images")
                                     .Where(x => x.FlightReservations.FirstOrDefault().Flight.Date > DateTime.Now && x.Count > 0)
                                     .ToList();


                    return Ok(new { Offers = offers });
                }// using
            }// end all offer get

            [HttpGet]
            [Route("{offerId}")]
            public IHttpActionResult GetOffer(int offerId)
            {
                using (var context = new ApplicationDbContext())
                {
                    Offer offer = context.Offers.Include("FlightReservations")
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
                                         .SingleOrDefault(x => x.Id == offerId);


                    return Ok(new { Offer = offer });
                }// using
            }// end get offer by id

            [HttpGet]
            [Route("{sourceCityId}/{destenationCityId}")]
            public IHttpActionResult GetOffersOptions(int sourceCityId, int destenationCityId)
            {
                using (var context = new ApplicationDbContext())
                {
                    List<Offer> offers = context.Offers.Include("FlightReservations")
                                     .Include("FlightReservations.Flight")
                                     .Include("flightBackReservations.Flight")
                                     .Include("FlightReservations.Flight.DestinationCity")
                                     .Include("FlightReservations.Flight.SourceCity")
                                     .Include("FlightBackReservations.Flight.DestinationCity")
                                     .Include("FlightBackReservations.Flight.SourceCity")
                                     .Include("FlightReservations.Flight.DestinationCity.Country")
                                     .Include("FlightReservations.Flight.SourceCity.Country")
                                     .Include("FlightBackReservations.Flight.DestinationCity.Country")
                                     .Include("FlightBackReservations.Flight.SourceCity.Country")
                                     .Include("FlightReservations.Flight.DestinationCity.Images")
                                     .Include("HotelReservations")
                                     .Include("HotelReservations.Room")
                                     .Include("HotelReservations.Room.Room")
                                     .Include("HotelReservations.Room.Hotel.Images")
                                       .Where(x => (x.FlightReservations.FirstOrDefault().Flight.Date > DateTime.Now && x.Count > 0)
                                       && ((x.FlightReservations.FirstOrDefault().Flight.SourceCity.Id == sourceCityId)
                                       && (x.FlightReservations.FirstOrDefault().Flight.DestinationCity.Id == destenationCityId)))
                                       .ToList();

                    return Ok(new { OffersOptions = offers });
                }// using
            }// end offer option from city to city

            [HttpPost]
            [Route("ReserveOffer")]
            public IHttpActionResult ReserveOffer([FromBody] ApiModelsView.OfferReservationModelView model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var user1 = model.Customer;
                ApplicationUser user = db.Users.Find(user1);

                
                Offer offer = db.Offers
                                          .Include("FlightReservations")
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
                                          .SingleOrDefault(x => x.Id == model.Id);
             

                // String flightClass = 

                if (offer.NewPrice > user.Credit)
                {

                    return BadRequest("Not Enough Credit");
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
                    return Ok(new { OfferConfermation = or });
                }

                return BadRequest();
            }

            [HttpGet]
            [Route("MyReservedOffers/{userId}")]
            public IHttpActionResult MyReservedOffers(string userId) 
            {
                ApplicationDbContext db = new ApplicationDbContext();
                List<OfferReservation> myOffers = db.OfferReservations.Include("Offer.FlightReservations")
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
                                                       .Include("Offer.HotelReservations.Room.Hotel.City.Images")
                                                       .Where(x => x.Customer.Id == userId).ToList();


                return Ok(new { MyOffers = myOffers });
            }

            [HttpGet]
            [Route("MyOfferReservation/{reservId}")]
            public IHttpActionResult MyOfferReservation(int reservId) 
            {
                ApplicationDbContext db = new ApplicationDbContext();
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
                                                       .Include("Offer.HotelReservations.Room.Hotel.City.Images")
                                                       .SingleOrDefault(x => x.Id == reservId);
                return Ok(new { MyOffer = offer });

            }

            [HttpPost]
            [Route("CncelReserveOffer")]
            public IHttpActionResult CancelReserveOffer([FromBody] ApiModelsView.OfferReservationModelView model)
            {
                try
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    var user1 = model.Customer;
                    ApplicationUser user = db.Users.Find(user1);

                    OfferReservation or = db.OfferReservations.Include("Offer")
                                                        .Include("Offer.FlightReservations")
                                                        .Include("Offer.FlightReservations.Customer")
                                                        .Include("Offer.FlightBackReservations")
                                                        .Include("Offer.FlightBackReservations.Customer")
                                                        .Include("Offer.HotelReservations")
                                                        .Include("Offer.HotelReservations.Guest")
                                                        .SingleOrDefault(x => x.Id == model.Id);


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

                    return Ok(new { Message = "the reservation canceld.." });
                }
                catch { }
                return BadRequest();

            }



        } // end offers api controller

        /// <summary>
        /// Message API Controller (Contact Us)
        /// </summary>
        [RoutePrefix("api/Message")]
        public class ApiMessageController : ApiController
        {
            [HttpPost]
            [Route("")]
            public IHttpActionResult SendMessage([FromBody] ApiModelsView.MessageModelView model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                try
                {
                    Message msg = new Message()
                    {
                        Email = model.Email,
                        Subject = model.Subject,
                        Text = model.Text,
                        Date_Time = DateTime.Now,
                        DisplayDate_Time = DateTime.Now.ToString(),
                        IsRead = false
                    };
                    db.Messages.Add(msg);
                    db.SaveChanges();
                    return Ok(new { Message = "Message send" });
                }
                catch
                {

                }
                return BadRequest();
            } // end send message method
        } // end api message controller

        /// <summary>
        /// Account API Controller
        /// </summary>
        [RoutePrefix("api/Account")]
        public class AccountController : ApiController
        {
            private AuthRepository _repo = null;

            public AccountController()
            {
                _repo = new AuthRepository();
            }

            // POST api/Account/Register
            [AllowAnonymous]
            [Route("Register")]
            public async Task<IHttpActionResult> Register(RegisterViewModel userModel)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IdentityResult result = await _repo.RegisterUser(userModel);
                IHttpActionResult errorResult = GetErrorResult(result);

                if (errorResult != null)
                {
                    return Ok(new { userModel = result });
                }

                return Ok(new { userModel = result });
            }

            [AllowAnonymous]
            [Route("Login")]
            public async Task<IHttpActionResult> Login([FromBody] LoginViewModel userModel)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IdentityUser result = await _repo.FindUser(userModel.UserName, userModel.Password);

                if (result == null)
                {
                    return BadRequest("Username or Password is Incorrect!!");
                }

                return Ok(new { userModel = result });
            }

            [AllowAnonymous]
            [Route("Update")]
            public async Task<IHttpActionResult> UpdateProfile([FromBody] ApiModelsView.UpdateAccountViewModel userModel)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IdentityResult result = await _repo.UpdateUser(userModel);
                if (result == null)
                {
                    return BadRequest("");
                }

                return Ok(new { Message = "user info Updated" });
            }

            [AllowAnonymous]
            [Route("ChangePassword")]
            public async Task<IHttpActionResult> ChangeMyPassword([FromBody] ApiModelsView.ChangePasswordViewModel model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                IdentityResult result = await _repo.ChangeMyPassword(model);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(new { Message = "PasswordChanged" });
            }

            [HttpPost]
            [Route("ForgetPassword")]
            public IHttpActionResult ResetMyPassword([FromBody] ApiModelsView.ResetPasswordViewModel model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool result = _repo.ResetMyPassword(model);
                if (result == false)
                {
                    return BadRequest();
                }

                return Ok(new { Message = "Password reset Successfully.." });
            }

            [HttpPost]
            [Route("ChargeCredit")]
            public IHttpActionResult ChargeCreditCard([FromBody] ApiModelsView.RechargeViewModel model)
            {
                ApplicationDbContext db = new ApplicationDbContext();
                string user1 = model.UserId;

                var user = db.Users.Find(user1);

                try
                {
                    if (model.NewCredit > 0)
                        user.Credit += model.NewCredit;
                    db.SaveChanges();

                    return Ok(new { userModel2 = user });
                }
                catch
                {

                }
                return BadRequest();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _repo.Dispose();
                }

                base.Dispose(disposing);
            }

            private IHttpActionResult GetErrorResult(IdentityResult result)
            {
                if (result == null)
                {
                    return InternalServerError();
                }

                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        foreach (string error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        // No ModelState errors are available to send, so just return an empty BadRequest.
                        return BadRequest();
                    }

                    return BadRequest(ModelState);
                }

                return null;
            }

          
           
        }   // end api Account controller


    } // end ApiWebServiceController/



} // end name space



