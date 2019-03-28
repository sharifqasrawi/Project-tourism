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
using TravelAndTourismWebsite.Repository;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace TravelAndTourismWebsite.Controllers
{

    public class AdminController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public AdminController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }


        public AdminController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        /*
         * Admin home page
         */
 
        [Authorize(Roles = "Admin,Flights Manager,Support,Hotel Manager,Offers Manager")]
        public ActionResult Index()
        {
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

            ViewBag.ExRateCount = ExRate;
            ViewBag.GRateCount = GRate;
            ViewBag.NbRateCount = NBRate;
            ViewBag.BRateCount = BRate;
            ViewBag.AllRates = allRates;

            return View();
        }

        //Countries management
        [Authorize(Roles = "Admin")]
        public ActionResult Countries(int? page, string Search)
        {
            IPagedList<Country> countries;

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    countries = db.Countries.Where(x => x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search))
                                                        .ToList()
                                                        .ToPagedList(page ?? 1, 15);
                    break;
                case "ar-SA":
                    countries = db.Countries.Where(x => x.Name_Ar.Contains(Search) || string.IsNullOrEmpty(Search))
                                                        .ToList()
                                                        .ToPagedList(page ?? 1, 15);
                    break;
                default:
                    countries = db.Countries.Where(x => x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search))
                                                        .ToList()
                                                        .ToPagedList(page ?? 1, 15);
                    break;
            }

            return View(countries);
        }

        /*
         * Add new country
         */

        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCountry()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCountry(Country model, HttpPostedFileBase uploadFile)
        {

            string path = "~/Images/no-thumbnail.png";
            string fileName = "";

            string flag = "";

            //Uploading multiple images

            try
            {
                if (checkFileType(uploadFile.FileName))
                {
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + uploadFile.FileName;
                    path = "~/Images/Flags/" + fileName;
                    uploadFile.SaveAs(Server.MapPath(path));
                    flag = path;
                }
            }
            catch
            {
                // ViewBag.ImageSizeError = Resource.ImageUploadError;
                return View(model);
            }



            Country c = new Country()
            {
                Name_En = model.Name_En,
                Name_Ar = model.Name_Ar,
                Visible = true,
                Flag = flag
            };
            db.Countries.Add(c);
            db.SaveChanges();

            return RedirectToAction("Countries", "Admin");

        }

        /*
         * Edit Country
         */
        [Authorize(Roles = "Admin")]
        public ActionResult EditCountry(int id)
        {
            return View(db.Countries.Find(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCountry(Country model, HttpPostedFileBase uploadFile)
        {

            Country c = db.Countries.Find(model.Id);
            c.Name_En = model.Name_En;
            c.Name_Ar = model.Name_Ar;

            string fileName = "";
            string flag = "";
            string path = "";
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                var ImagePath = Server.MapPath(c.Flag);

                if (System.IO.File.Exists(ImagePath))
                    System.IO.File.Delete(ImagePath);

                if (checkFileType(uploadFile.FileName))
                {
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + uploadFile.FileName;
                    path = "~/Images/Flags/" + fileName;
                    uploadFile.SaveAs(Server.MapPath(path));
                    flag = path;
                }
                c.Flag = flag;
            }

            db.SaveChanges();

            return RedirectToAction("Countries", "Admin");

        }

        /*
         * Delete country
         */

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCountry(int id)
        {
            Country c = db.Countries.Find(id);
            db.Countries.Remove(c);
            db.SaveChanges();
            return RedirectToAction("Countries", "Admin");
        }


        /*
         * Change country visibility
         */
        [Authorize(Roles = "Admin")]
        public PartialViewResult ChangeCountryVisibility(int id)
        {
            //Getting the country
            Country country = db.Countries.Find(id);

            if (country.Visible)
            {
                country.Visible = false;
            }
            else
            {
                country.Visible = true;
            }
            db.SaveChanges();

            return PartialView("_ChangeCountryVisibilityPartial", country);
        }

        //////////////////////////////////////

        // Cities management
        [Authorize(Roles = "Admin")]
        public ActionResult Cities(int? page, int? Country)
        {
            List<City> cities = db.Cities.Include("Country")
                                         .Where(x => (x.Country.Id == Country || Country == null))
                                         .ToList();


            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
            }
            return View(cities.ToPagedList(page ?? 1, 15));
        }

        /*
         * Add new city
         */

        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCity(int CountryId)
        {
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
            }
            ViewBag.Country = CountryId;
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewCity(City model, HttpPostedFileBase[] uploadFile)
        {
            try
            {
                string path = "~/Images/no-thumbnail.png";
                string fileName = "";

                //Creating a list of images
                List<CityImage> Images = new List<CityImage>();

                //Uploading multiple images
                if (uploadFile[0] != null)
                {
                    try
                    {
                        foreach (HttpPostedFileBase file in uploadFile)
                        {
                            if (file != null && checkFileType(file.FileName))
                            {
                                fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + file.FileName;
                                path = "~/Images/Cities/" + fileName;
                                file.SaveAs(Server.MapPath(path));
                                Images.Add(new CityImage() { Path = path });
                            }
                        }
                    }
                    catch
                    {
                        // ViewBag.ImageSizeError = Resource.ImageUploadError;
                        return View(model);
                    }
                }
                else
                {
                    Images.Add(new CityImage() { Path = path });
                }

                City city = new City()
                {
                    Country = db.Countries.Find(int.Parse(Request["Country"].ToString())),
                    Name_En = model.Name_En,
                    Name_Ar = model.Name_Ar,
                    Description_En = model.Description_En,
                    Description_Ar = model.Description_Ar,
                    CityLocation = model.CityLocation
                };
                foreach (var p in Images)
                {
                    p.City = city;
                }
                city.Images = Images;
                db.Cities.Add(city);
                db.Countries.Find(int.Parse(Request["Country"].ToString())).Cities.Add(city);
                db.SaveChanges();

                return RedirectToAction("Cities", "Admin", new { Country = int.Parse(Request["Country"]) });
            }
            catch
            {

            }
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
            }
            return View(model);
        }


        /*
         * Edit city
         */

        [Authorize(Roles = "Admin")]
        public ActionResult EditCity(int id)
        {
            City city = db.Cities.Include("Images").SingleOrDefault(x => x.Id == id);
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", city.Country.Id);
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", city.Country.Id);
                    break;
                default:
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", city.Country.Id);
                    break;
            }
            return View(city);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditCity(City model, HttpPostedFileBase[] uploadFile)
        {
            try
            {
                City city = db.Cities.Include("Images").Include("Country").SingleOrDefault(x => x.Id == model.Id);
                city.Country = db.Countries.Find(int.Parse(Request["Country"].ToString()));
                city.Name_En = model.Name_En;
                city.Name_Ar = model.Name_Ar;
                city.Description_En = model.Description_En;
                city.Description_Ar = model.Description_Ar;
                city.CityLocation = model.CityLocation;

                string path = "";
                string fileName = "";

                //Creating a list of images
                List<CityImage> Images = new List<CityImage>();

                //Uploading multiple images
                if (uploadFile[0] != null)
                {
                    foreach (var g in city.Images)
                    {
                        var ImagePath = Server.MapPath(g.Path);

                        if (System.IO.File.Exists(ImagePath))
                            System.IO.File.Delete(ImagePath);
                    }
                    try
                    {
                        foreach (HttpPostedFileBase file in uploadFile)
                        {
                            if (file != null && checkFileType(file.FileName))
                            {
                                fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + file.FileName;
                                path = "~/Images/Cities/" + fileName;
                                file.SaveAs(Server.MapPath(path));
                                Images.Add(new CityImage() { Path = path });
                            }
                        }

                        city.Images = Images;
                    }
                    catch
                    {
                        switch (SiteLanguages.GetCurrentLanguageCulture())
                        {
                            case "en-us":
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", city.Country.Id);
                                break;
                            case "ar-SA":
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", city.Country.Id);
                                break;
                            default:
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", city.Country.Id);
                                break;
                        }
                        // ViewBag.ImageSizeError = Resource.ImageUploadError;
                        return View(city);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Cities", "Admin", new { Country = city.Country.Id });
            }
            catch
            {

            }
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-us":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", model.Country.Id);
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", model.Country.Id);
                    break;
                default:
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", model.Country.Id);
                    break;
            }
            return View(model);
        }

        /*
         * Delete city
         */

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCity(int id)
        {
            City city = db.Cities.Include("Country").SingleOrDefault(x => x.Id == id);
            int cid = city.Country.Id;
            List<CityImage> cityImgs = db.CityImages.Where(x => x.City.Id == city.Id).ToList();
            foreach (var ci in cityImgs)
            {
                db.CityImages.Remove(ci);
            }
            db.Cities.Remove(city);
            db.SaveChanges();
            return RedirectToAction("Cities", "Admin", new { Country = cid });
        }
        //////////////////////////////////////

        /*
         * Flights management
         */

        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult Flights(int? page,int? SourceCity, int? DestinationCity, DateTime? Date)
        {
            IPagedList<Flight> flights = db.Flights.Include("SourceCity")
                                                  .Include("DestinationCity")
                                                  .Include("Reservations")
                                                  .Where(x => (x.SourceCity.Id == SourceCity || SourceCity == null)
                                                            && (x.DestinationCity.Id == DestinationCity || DestinationCity == null)
                                                            && (x.Date == Date || Date == null))
                                                  .ToList()
                                                  .ToPagedList(page ?? 1, 15);


            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
            }

            return View(flights);
        }

        /*
         * Add a flight
         */

        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult AddFlight()
        {
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
            }
            return View();

        }
        [HttpPost]
        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult AddFlight(Flight model)
        {

            City s = db.Cities.Find(int.Parse(Request["SourceCity"].ToString()));
            City d = db.Cities.Find(int.Parse(Request["DestinationCity"].ToString()));

            try
            {

                Flight f = new Flight()
                {
                    SourceCity = s,
                    DestinationCity = d,
                    EcoSeatsCount = model.EcoSeatsCount,
                    FrstSeatsCount = model.FrstSeatsCount,
                    Date = Convert.ToDateTime(model.Date),
                    DisplayDate = Convert.ToDateTime(model.Date).ToShortDateString(),
                    Time = model.Time,
                    FlightDuration = model.FlightDuration,
                    EconomyTicketPrice = model.EconomyTicketPrice,
                    FirstClassTicketPrice = model.FirstClassTicketPrice,
                    Airline = model.Airline
                };
                db.Flights.Add(f);
                db.SaveChanges();

                return RedirectToAction("Flights", "Admin");
            }
            catch
            {

            }
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
            }
            return View(model);
        }

        /*
        * Edit a flight
        */
        public ActionResult EditFlight(int id)
        {
            Flight flight = db.Flights.Include("SourceCity")
                                      .Include("DestinationCity")
                                      .SingleOrDefault(x => x.ID == id);

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.DestinationCity.Id);
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar", flight.DestinationCity.Id);
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.DestinationCity.Id);
                    break;
            }

            return View(flight);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult EditFlight(Flight model)
        {
            Flight flight = db.Flights.Include("SourceCity")
                                      .Include("DestinationCity")
                                      .SingleOrDefault(x => x.ID == model.ID);
            try
            {
                flight.SourceCity = db.Cities.Find(int.Parse(Request["SourceCity"].ToString()));
                flight.DestinationCity = db.Cities.Find(int.Parse(Request["DestinationCity"].ToString()));
                flight.Date = model.Date;
                flight.DisplayDate = flight.Date.ToShortDateString();
                flight.EconomyTicketPrice = model.EconomyTicketPrice;
                flight.FirstClassTicketPrice = model.FirstClassTicketPrice;
                flight.FlightDuration = model.FlightDuration;
                flight.EcoSeatsCount = model.EcoSeatsCount;
                flight.FrstSeatsCount = model.FrstSeatsCount;
                flight.Time = model.Time;

                db.SaveChanges();
                return RedirectToAction("Flights", "Admin");
            }
            catch
            {

            }

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.DestinationCity.Id);
                    break;
                case "ar-SA":
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar", flight.DestinationCity.Id);
                    break;
                default:
                    ViewBag.SourceCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.SourceCity.Id);
                    ViewBag.DestinationCity = new SelectList(db.Cities.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En", flight.DestinationCity.Id);
                    break;
            }
            return View(model);
        }

        /*
        * Delete a flight
        */

        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult DeleteFlight(int id)
        {
            Flight flight = db.Flights.Find(id);
            db.Flights.Remove(flight);
            db.SaveChanges();
            return RedirectToAction("Flights", "Admin");
        }

        [Authorize(Roles = "Admin,Flights Manager")]
        public ActionResult FlightPsngrsLst(int? page, int id)
        {
            IPagedList<FlightReservation> rsrvtions = db.FlightReservations.Include("Flight")
                                                                           .Include("Flight.SourceCity")
                                                                           .Include("Flight.DestinationCity")
                                                                           .Include("Customer")
                                                                           .Where(x => x.Flight.ID == id && x.Customer != null)
                                                                           .ToList()
                                                                           .ToPagedList(page ?? 1, 15);

            

            return View(rsrvtions);
        }

        /*
        * Users management
        */

        [Authorize(Roles = "Admin")]
        public ActionResult UsersManagement(int? page, string Search)
        {
            List<ApplicationUser> users = db.Users.Where(x => (x.UserName.Contains(Search.Trim())
                                                                || x.FirstName.Contains(Search.Trim())
                                                                || x.LastName.Contains(Search.Trim())
                                                                || x.Email.Contains(Search.Trim())
                                                                || x.PhoneNumber.Contains(Search.Trim())
                                                                || String.IsNullOrEmpty(Search)))
                                                                .OrderByDescending(x => x.Active)
                                                                .ThenBy(x => x.UserName)
                                                                .ToList();
            return View(users.ToPagedList(page ?? 1, 10));
        }

        /*
       * Filter users by account status 
       */

        [Authorize(Roles = "Admin")]
        public ActionResult FilterUsers(int? page, string Status)
        {
            try
            {
                var status = false;
                if (Status == "All")
                {
                    return RedirectToAction("UsersManagement");
                }
                else
                {
                    switch (Status)
                    {
                        case "Active":
                            status = true;
                            break;
                        case "Blocked":
                            status = false;
                            break;
                    }

                    IPagedList<ApplicationUser> users = db.Users.Where(x => x.Active.Equals(status))
                                                          .ToList()
                                                          .ToPagedList(page ?? 1, 10);
                    return View("UsersManagement", users);
                }
            }
            catch { }
            return RedirectToAction("UsersManagement");
        }

        /*
       * Filter users by account type
       */

        [Authorize(Roles = "Admin")]
        public ActionResult FilterUsers2(int? page, string Type)
        {
            try
            {
                var type = false;
                if (Type == "All")
                {
                    return RedirectToAction("UsersManagement");
                }
                else
                {
                    switch (Type)
                    {
                        case "Admin":
                            type = true;
                            break;
                        case "User":
                            type = false;
                            break;
                    }

                    IPagedList<ApplicationUser> users = db.Users.Where(x => x.IsAdmin.Equals(type))
                                                          .ToList()
                                                          .ToPagedList(page ?? 1, 10);
                    return View("UsersManagement", users);
                }
            }
            catch { }
            return RedirectToAction("UsersManagement");
        }

        /*
       * Add new admin user
       */

        [Authorize(Roles = "Admin")]
        public ActionResult AddNewUser()
        {
            List<Country> countries = db.Countries.OrderBy(x => x.Name_En).Where(x => x.Visible).ToList();


            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddNewUser(RegisterViewModel model)
        {

            List<Country> countries = db.Countries.OrderBy(x => x.Name_En).Where(x => x.Visible).ToList();
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Country = new SelectList(countries, "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Country = new SelectList(countries, "Id", "Name_En");
                    break;
            }

            if (ModelState.IsValid)
            {
                //Checking if the email is already used
                var user = db.Users.Where(x => x.Email == model.Email).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.Message = Resource.EmailUsed;
                    ViewBag.Alert = "alert alert-danger";
                    return View(model);
                }

                //Checking if the username is already used
                user = db.Users.Where(x => x.UserName == model.UserName).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.Message = Resource.UsernameUsed;
                    ViewBag.Alert = "alert alert-danger";
                    return View(model);
                }

                //Creating a new user
                user = new ApplicationUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Country = db.Countries.Find(int.Parse(model.Country)).Name_En,
                    City = db.Cities.Find(int.Parse(model.City)).Name_En,
                    UserName = model.UserName,
                    Active = true,
                    LoginCount = 0,
                    RegistrationDateTime = DateTime.Now,
                    IsAdmin = true
                };
                var result = await UserManager.CreateAsync(user, model.Password);


                switch (SiteLanguages.GetCurrentLanguageCulture())
                {
                    case "en-US":
                        user.PreferredInterfaceLanguage = "English";
                        break;
                    case "ar-SA":
                        user.PreferredInterfaceLanguage = "Arabic";
                        break;
                    default:
                        user.PreferredInterfaceLanguage = "English";
                        break;
                }
                db.SaveChanges();

                return RedirectToAction("UsersManagement", "Admin");
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /*
       * Users roles managment
       */

        [Authorize(Roles = "Admin")]
        public ActionResult UserRolesManagement(string Id)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            //Getting list of user's roles
            var userRoles = userManager.GetRoles(Id).ToList();

            ViewBag.User = db.Users.Find(Id);
            //Getting list of all available roles
            ViewBag.Roles = new SelectList(db.Roles.OrderBy(x => x.Name).ToList(), "Name", "Name");

            return View(userRoles);
        }

        /*
       * Add role to user
       */

        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles_AddToRole(string Id)
        {
            try
            {
                //Getting the requested role
                string role = Request["Roles"].ToString();

                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                //Adding the user to the role
                userManager.AddToRole(Id, role);

                return RedirectToAction("UserRolesManagement", new { Id = Id });
            }
            catch
            {

            }
            return RedirectToAction("UserRolesManagement", new { Id = Id });
        }


        /*
       * Remove role from user
       */

        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRoles_RemoveFromRole(string Id, string role)
        {
            try
            {
                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(Id, role);

                return RedirectToAction("UserRolesManagement", new { Id = Id });
            }
            catch
            {

            }
            return RedirectToAction("UserRolesManagement", new { Id = Id });
        }

        [Authorize(Roles = "Admin")]
        public PartialViewResult BlockUnblockUser(string Id)
        {
            ApplicationUser user = db.Users.Find(Id);
            if (user.Active)
            {
                user.Active = false;
            }
            else
            {
                user.Active = true;
            }
            db.SaveChanges();


            return PartialView("_UserAccountStatusPartial", user);
        }

        /*
       * View user information
       */

        [Authorize(Roles = "Admin")]
        public ActionResult UserInfo(string Id)
        {
            ApplicationUser user = db.Users.Find(Id);
            return View(user);
        }

        /*
       * Messages
       */
        [Authorize(Roles = "Admin,Support")]
        public ActionResult Messages(int? page, string Search)
        {
            List<Message> messages = db.Messages.Where(x => x.Subject.Contains(Search)
                                                         || x.Email.Contains(Search)
                                                         || string.IsNullOrEmpty(Search))
                                                .OrderBy(x => x.IsRead)
                                                .ThenByDescending(x => x.Date_Time)
                                                .ToList();

            return View(messages.ToPagedList(page ?? 1, 10));
        }

        /*
       * View message
       */

        [Authorize(Roles = "Admin,Support")]
        public ActionResult ViewMessage(int Id)
        {
            Message msg = db.Messages.Find(Id);
            msg.IsRead = true;
            db.SaveChanges();
            return View(msg);
        }

        /*
       * Reply to message
       */

        [Authorize(Roles = "Admin,Support")]
        public ActionResult MessageReply(string Email, string Subject)
        {
            ReplyMessageViewModel rp = new ReplyMessageViewModel()
            {
                Email = Email,
                Subject = Subject
            };

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    rp.MessageText = "\n\n\n\n\n\n\n\n\n\n" +
                                   "-------------------------------------------\n" +
                                   "Travel And Tourism website\n" +
                                   "Admin: " + User.Identity.GetUserName().ToString();
                    break;
                case "ar-SA":
                    rp.MessageText = "\n\n\n\n\n\n\n\n\n\n" +
                                   "-------------------------------------------\n" +
                                   "موقع السياحة والسفر\n" +
                                   "المنظم: " + User.Identity.GetUserName().ToString();
                    break;
                default:
                    rp.MessageText = "\n\n\n\n\n\n\n\n\n\n" +
                                   "-------------------------------------------\n" +
                                   "Travel And Tourism website\n" +
                                   "Admin: " + User.Identity.GetUserName().ToString();
                    break;
            }


            return View(rp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Support")]
        public ActionResult MessageReply(ReplyMessageViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Email email = new Email(model.Email, model.Subject, model.MessageText);
                    email.Send();

                    return RedirectToAction("Messages", "Admin");
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {

            }
            ViewBag.ErrorSendingEmail = Resource.ErrorSendingEmail;
            ViewBag.ErrorAlert = "alert";
            return View(model);
        }

        [Authorize(Roles = "Admin,Support")]
        public ActionResult SendEmailToUsers()
        {
            return View();
        }

        /*
       * Send email to each user in the system
       */

        [HttpPost]
        [Authorize(Roles = "Admin,Support")]
        public ActionResult SendEmailToUsers(EmailToAllUsers model)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            users = db.Users.ToList();
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var user in users)
                    {
                        Email email = new Email(user.Email, model.Subject, model.EmailText);
                        email.Send();
                    }
                    return RedirectToAction("Messages", "Admin");
                }
                else
                {
                    return View(model);
                }
            }
            catch
            { }
            ViewBag.Message = Resource.ErrorSendingEmail;
            ViewBag.Alert = "alert alert-danger";
            return View(model);
        }

        /*
       * Get hotel report
       */
        [Authorize(Roles="Admin, Hotel Manager")]
        public ActionResult Reports()
        {
            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.Hotels = new SelectList(db.Hotels.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    ViewBag.Hotels = new SelectList(db.Hotels.OrderBy(x => x.Name_Ar).ToList(), "Id", "Name_Ar");
                    break;
                default:
                    ViewBag.Hotels = new SelectList(db.Hotels.OrderBy(x => x.Name_En).ToList(), "Id", "Name_En");
                    break;
            }
            
            
            return View();
        }

        [Authorize(Roles = "Admin, Hotel Manager")]
        public PartialViewResult Report(int Hotels)
        {
            List<HotelReservation> hrs = db.HotelReservations.Include("Guest")
                                                             .Include("Room")
                                                             .Include("Room.Hotel")
                                                             .Where(x => x.Room.Hotel.Id == Hotels)
                                                             .ToList();

            ViewBag.SweetsCount = db.HotelRooms.Where(x => x.Hotel.Id == Hotels && x.Room.Type == "Sweet").Sum(x => x.Count);
            ViewBag.RoomsCount = db.HotelRooms.Where(x => x.Hotel.Id == Hotels && x.Room.Type == "Room").Sum(x => x.Count);

            return PartialView("_ReportPartial", hrs);
        }

        /*
       * Cancel hotel reservation by admin
       */

        [Authorize(Roles = "Admin")]
        public ActionResult CancelHotelReservation(int Id, string uid)
        {
            HotelReservation hr = db.HotelReservations.Find(Id);

            ApplicationUser user = db.Users.Find(uid);

            user.Credit += float.Parse(hr.ReservationCost.ToString());


            db.HotelReservations.Remove(hr);
            db.SaveChanges();

            return RedirectToAction("Reports", "Admin");
        }
    }
}