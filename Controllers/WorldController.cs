using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelAndTourismWebsite.Models;
using TravelAndTourismWebsite.Resources;
using TravelAndTourismWebsite.WebsiteLanguages;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace TravelAndTourismWebsite.Controllers
{
    public class WorldController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /*
         * View all countries 
         */
        [AllowAnonymous]
        public ActionResult Countries(int? page, string Search)
        {
            List<Country> countries;
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    countries = db.Countries.Where(x => x.Name_En.Contains(Search) || String.IsNullOrEmpty(Search) && x.Visible == true)
                                            .OrderBy(x => x.Name_En)     
                                            .ToList();
                    break;
                case "ar-SA":
                    countries = db.Countries.Where(x => x.Name_Ar.Contains(Search) || String.IsNullOrEmpty(Search) && x.Visible == true)
                                                .OrderBy(x => x.Name_Ar)  
                                                .ToList();
                    break;
                default:
                    countries = db.Countries.Where(x => x.Name_En.Contains(Search) || String.IsNullOrEmpty(Search) && x.Visible == true)
                                                .OrderBy(x => x.Name_En)  
                                                .ToList();
                    break;
            }
            if (Session["NoCities"] != null)
                ViewBag.NoCities = Session["NoCities"].ToString();

            return View(countries.ToPagedList(page ?? 1, 12));
        }
        /*
         * View cities in a country
         */

        [AllowAnonymous]
        public ActionResult Cities(int? page, int CountryId, string Search)
        {
            List<City> cities;
            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    cities = db.Cities.Include("Country")
                                         .Include("Images")
                                         .Where(x => x.Country.Id == CountryId 
                                                &&(x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search)))
                                         .OrderBy(x => x.Name_En)
                                         .ToList();
                    break;
                case "ar-SA":
                    cities = db.Cities.Include("Country")
                                        .Include("Images")
                                        .Where(x => x.Country.Id == CountryId
                                               && (x.Name_Ar.Contains(Search) || string.IsNullOrEmpty(Search)))
                                        .OrderBy(x => x.Name_Ar)
                                        .ToList();
                    break;
                default:
                    cities = db.Cities.Include("Country")
                                         .Include("Images")
                                         .Where(x => x.Country.Id == CountryId
                                                && (x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search)))
                                         .OrderBy(x => x.Name_En)
                                         .ToList();
                    break;
            }

            if(cities.Count == 0)
            {
                Session.Add("NoCities", "No Cities");
                return RedirectToAction("Countries");
            }
            Session.Remove("NoCities");

            Country c = db.Countries.Find(CountryId);
            c.Rate++;
            db.SaveChanges();

            return View(cities.ToPagedList(page ?? 1, 12));
        }

        /*
         * Preview a city
         */
        [AllowAnonymous]
        public ActionResult CityPreview(int Cityid)
        {
            
            City city = db.Cities.Include("Country")
                                 .Include("Images")
                                 .SingleOrDefault(x => x.Id == Cityid);


            List<Flight> flights = db.Flights.Include("SourceCity").Include("DestinationCity")
                                                .Where(x => x.DestinationCity.Id == Cityid
                                                && x.Date > DateTime.Now)
                                                .ToList();

            List<Hotel> hotels = db.Hotels.Include("City")
                                          .Include("Images")
                                          .Where(x => x.City.Id == Cityid)
                                          .ToList();

            CityPreviewViewModel cp = new CityPreviewViewModel()
            {
                City = city,
                Flights = flights,
                Hotels = hotels
            };

            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.SourceCity = new SelectList(db.Cities.Where(x => x.Id != Cityid).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.Where(x => x.Id != Cityid).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.Where(x => x.Id != Cityid).ToList(), "Id", "Name_En");
                    break;
            }

            List<string> stars = new List<string>();
            stars.Add("*");
            stars.Add("**");
            stars.Add("***");
            stars.Add("****");
            stars.Add("*****");
            stars.Add("******");
            stars.Add("*******");

            ViewBag.Stars = new SelectList(stars);

            ViewBag.Airline = new SelectList(db.Flights.Select(x => x.Airline).Distinct().ToList());

            return View(cp);
        }

        /*
         * Get Flights going to a city
         */
        [AllowAnonymous]
        public PartialViewResult GetFlightsToCity(int Cityid, int SourceCity, string Airline)
        {


            List<Flight> flights = db.Flights.Include("SourceCity").Include("DestinationCity")
                                               .Where(x => x.DestinationCity.Id == Cityid
                                                        && (x.SourceCity.Id == SourceCity || SourceCity == null)
                                                        &&(x.Airline == Airline || string.IsNullOrEmpty(Airline)))
                                               .ToList();

            City city = db.Cities.Include("Country")
                                 .Include("Images")
                                 .SingleOrDefault(x => x.Id == Cityid);

            CityPreviewViewModel cp = new CityPreviewViewModel()
            {
                City = city,
                Flights = flights
            };

            
            return PartialView("_CityFlightsPartial", cp);
        }

        /*
         * Get all hotels in a city
         */
        [AllowAnonymous]
        public PartialViewResult GetCityHotels(int Cityid, string Stars, string HotelName)
        {

            List<Hotel> hotels = db.Hotels.Include("Images")
                                          .Where(x => (x.Stars == Stars.Length || string.IsNullOrEmpty(Stars))
                                                && x.City.Id == Cityid)
                                          .ToList();
            
            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    hotels = hotels.Where(x => x.Name_En.ToLower().Contains(HotelName.ToLower()) || string.IsNullOrEmpty(HotelName))
                                   .OrderBy(x => x.Name_En)
                                   .ToList();
                    break;
                case "ar-SA":
                    hotels = hotels.Where(x => x.Name_Ar.Contains(HotelName) || string.IsNullOrEmpty(HotelName))
                                 .OrderBy(x => x.Name_Ar)
                                 .ToList();
                    break;
                default:
                    hotels = hotels.Where(x => x.Name_En.ToLower().Contains(HotelName.ToLower()) || string.IsNullOrEmpty(HotelName))
                                 .OrderBy(x => x.Name_En)
                                 .ToList();
                    break;
            }

            City city = db.Cities.Include("Country")
                                 .Include("Images")
                                 .SingleOrDefault(x => x.Id == Cityid);

            CityPreviewViewModel cp = new CityPreviewViewModel()
            {
                City = city,
                Hotels = hotels
            };

            return PartialView("_CityHotelsPartial", cp);
        }
	}
}