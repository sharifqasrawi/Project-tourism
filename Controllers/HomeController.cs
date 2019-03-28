using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelAndTourismWebsite.Models;
using Microsoft.AspNet.Identity;

namespace TravelAndTourismWebsite.Controllers
{
    public class HomeController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        /*
         * Home page
         */
        public ActionResult Index()
        {
            List<Offer> offers = db.Offers.Include("FlightReservations")
                                          .Include("FlightReservations.Flight")
                                          .Include("FlightReservations.Flight.DestinationCity")
                                          .Include("FlightReservations.Flight.DestinationCity.Images")
                                          .Include("FlightReservations.Flight.DestinationCity.Country")
                                          .Include("HotelReservations")
                                          .Where(x => x.FlightReservations.FirstOrDefault().Flight.Date > DateTime.Now && x.Count > 0 && x.Status == "Available")
                                          .Take(2)
                                          .ToList();


            int ExRate = db.WebsiteRatings.Where(x => x.Rate == "Excellent").Count();
            int GRate = db.WebsiteRatings.Where(x => x.Rate == "Good").Count();
            int NBRate = db.WebsiteRatings.Where(x => x.Rate == "Not Bad").Count();
            int BRate = db.WebsiteRatings.Where(x => x.Rate == "Bad").Count();

            int allRates = db.WebsiteRatings.Count();

            float GPer = (float)(GRate * 100) / allRates;
            float ExPer = (float)(ExRate * 100) / allRates;
            float NbPer = (float)(NBRate * 100) / allRates;
            float BPer = (float)(BRate * 100) / allRates;

            ViewBag.GRate = GPer;
            ViewBag.ExRate = ExPer;
            ViewBag.NbRate = NbPer;
            ViewBag.BRate = BPer;

            HomeViewModel home = new HomeViewModel()
            {
                Countries = db.Countries.Include("Cities").Include("Cities.Images").Where(x => x.Visible == true)
                .OrderByDescending(x => x.Rate)
                .Take(5).ToList(),
                Hotels = db.Hotels.Include("Images").OrderByDescending(x => x.Rate).Take(5).ToList(),
                Offers = offers
            };

            return View(home);
        }

        /*
         * Website rating
         */
        [Authorize(Roles="User")]
        public PartialViewResult RateWebsite(string Rate)
        {
            string userId = User.Identity.GetUserId();

            WebsiteRating wr = db.WebsiteRatings.SingleOrDefault(x =>  x.User.Id == userId);

            if (wr == null)
            {
                wr = new WebsiteRating()
                {
                    User = db.Users.Find(User.Identity.GetUserId()),
                    Rate = Rate
                };
                db.WebsiteRatings.Add(wr);
            }
            else
            {
                wr.Rate = Rate;
            }
            db.SaveChanges();

            int ExRate = db.WebsiteRatings.Where(x => x.Rate == "Excellent").Count();
            int GRate = db.WebsiteRatings.Where(x => x.Rate == "Good").Count();
            int NBRate = db.WebsiteRatings.Where(x => x.Rate == "Not Bad").Count();
            int BRate = db.WebsiteRatings.Where(x => x.Rate == "Bad").Count();

            int allRates = db.WebsiteRatings.Count();

            float GPer = (float)(GRate * 100) / allRates;
            float ExPer = (float)(ExRate * 100) / allRates;
            float NbPer = (float)(NBRate * 100) / allRates;
            float BPer = (float)(BRate * 100) / allRates;

            ViewBag.GRate = GPer;
            ViewBag.ExRate = ExPer;
            ViewBag.NbRate = NbPer;
            ViewBag.BRate = BPer;

            return PartialView("_WebsiteRatingPartial");
        }
        /*
         * About page
         */
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        /*
         * Contact page
         */
        public ActionResult Contact()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Contact(Message model)
        {
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

                return RedirectToAction("Index", "Home");
            }
            catch
            {

            }
            return View(model);
        }
    }
}