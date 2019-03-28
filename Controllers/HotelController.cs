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
using System.Threading;

namespace TravelAndTourismWebsite.Controllers
{
    public class HotelController : BaseController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /*
         * Rooms management
         */
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult Rooms(int? page)
        {
            List<Room> rooms = db.Rooms.ToList();
            return View(rooms.ToPagedList(page ?? 1, 10));
        }

        /*
         * Creating new room
         */

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult CreateRoom()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult CreateRoom(Room model,string Type)
        {
            if(ModelState.IsValid)
            {
                Room room = new Room()
                {
                    Type = Type,
                    Cust_Count = model.Cust_Count,
                    Details_En = model.Details_En,
                    Details_Ar = model.Details_Ar,
                    Name = Type + " - " + model.Cust_Count.ToString()
                };
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Rooms", "Hotel");
            }
            return View(model);
        }

        /*
         * Edit Room
         */

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditRoom(int Id)
        {
            Room room = db.Rooms.Find(Id);
            return View(room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditRoom(Room model,string Type)
        {
            try
            {
                Room room = db.Rooms.Find(model.Id);
                room.Name = Type + " - " + model.Cust_Count;
                room.Type = Type;
                room.Details_En = model.Details_En;
                room.Details_Ar = model.Details_Ar;
                room.Cust_Count = model.Cust_Count;
                db.SaveChanges();

                return RedirectToAction("Rooms", "Hotel");
            }
            catch
            {

            }
            return View(model);
        }

        /*
         * Delete room
         */

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult DeleteRoom(int Id)
        {
            Room room = db.Rooms.Find(Id);
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Rooms", "Hotel");
        }

        /*
         * Add new hotel
         */
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult AddNewHotel()
        {
            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                  //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                  //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_Ar");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");
                    break;
                default:
                  //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
            }
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult AddNewHotel(Hotel model, HttpPostedFileBase[] uploadFile)
        {
            try
            {
                string path = "~/Images/no-thumbnail.png";
                string fileName = "";

                //Creating a list of images
                List<HotelImage> Images = new List<HotelImage>();

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
                                path = "~/Images/Hotels/" + fileName;
                                file.SaveAs(Server.MapPath(path));
                                Images.Add(new HotelImage() { Path = path });
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
                    Images.Add(new HotelImage() { Path = path });
                }


                Hotel hotel = new Hotel()
                {
                    Name_En = model.Name_En,
                    Name_Ar = model.Name_Ar,
                    Details_En = model.Details_En,
                    Details_Ar = model.Details_Ar,
                    Email = model.Email,
                    Website = model.Website,
                    PhoneNumber = model.PhoneNumber,
                    Stars = model.Stars,
                    Location = model.Location,
                    GpsX =model.GpsX,
                    GpsY=model.GpsY,
                    City = db.Cities.Find(int.Parse(Request["City"].ToString()))
                };

                foreach (var p in Images)
                {
                    p.Hotel = hotel;
                }
                hotel.Images = Images;

                db.Hotels.Add(hotel);
                db.SaveChanges();

                return RedirectToAction("HotelsManagement", "Hotel");
            }
            catch
            {

            }

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
                case "ar-SA":
                    //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_Ar");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");
                    break;
                default:
                    //  ViewBag.city = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");
                    break;
            }
            return View(model);
        }

        /*
         * Edit hotel
         */

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditHotel(int Id)
        {
            Hotel hotel = db.Hotels.Include("City")
                                   .Include("City.Country")
                                   .Include("Images")
                                   .Include("HotelRooms")
                                   .SingleOrDefault(x => x.Id == Id);

            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                    break;
                case "ar-SA":
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_Ar", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", hotel.City.Country.Id);
                    break;
                default:
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                    break;
            }

            return View(hotel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditHotel(Hotel model, HttpPostedFileBase[] uploadFile)
        {
            Hotel hotel = db.Hotels.Include("City")
                                      .Include("City.Country")
                                      .Include("Images")
                                      .SingleOrDefault(x => x.Id == model.Id);
            try
            {

                hotel.Name_En = model.Name_En;
                hotel.Name_Ar = model.Name_Ar;
                hotel.Details_En = model.Details_En;
                hotel.Details_Ar = model.Details_Ar;
                hotel.Stars = model.Stars;
                hotel.Website = model.Website;
                hotel.Location = model.Location;
                hotel.GpsX = model.GpsX;
                hotel.GpsY = model.GpsY;
                hotel.Email = model.Email;
                hotel.PhoneNumber =model.PhoneNumber;
                hotel.City = db.Cities.Find(int.Parse(Request["city"].ToString()));


                string path = "";
                string fileName = "";

                //Creating a list of images
                List<HotelImage> Images = new List<HotelImage>();

                //Uploading multiple images
                if (uploadFile[0] != null)
                {
                    foreach (var g in hotel.Images)
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
                                path = "~/Images/Hotels/" + fileName;
                                file.SaveAs(Server.MapPath(path));
                                Images.Add(new HotelImage() { Path = path });
                            }
                        }

                        hotel.Images = Images;
                    }
                    catch
                    {
                        switch (SiteLanguages.GetCurrentLanguageCulture())
                        {
                            case "en-US":
                                ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                                break;
                            case "ar-SA":
                                ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_Ar", hotel.City.Id);
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", hotel.City.Country.Id);
                                break;
                            default:
                                ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                                ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                                break;
                        } 
                        return View(hotel);
                    }

                }

                db.SaveChanges();

                return RedirectToAction("HotelsManagement", "Hotel");
            }
            catch
            {

            }
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                    break;
                case "ar-SA":
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_Ar", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar", hotel.City.Country.Id);
                    break;
                default:
                    ViewBag.city = new SelectList(db.Cities.Where(x => x.Country.Id == hotel.City.Country.Id).ToList(), "Id", "Name_En", hotel.City.Id);
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En", hotel.City.Country.Id);
                    break;
            }
            return View(model);
        }

        /*
         * View all Hotels
         */

        [AllowAnonymous]
        public ActionResult AllHotels(int? page, string HotelName, int? Country, int? City, string Stars)
        {
            //HotelName = "";
            List<Hotel> hotels = db.Hotels.Include("City")
                                          .Include("City.Country")
                                          .Include("Images")
                                          .Include("HotelRooms")
                                          .Where(x => (x.City.Country.Id == Country || Country==null)
                                                   && (x.City.Id == City || City == null)
                                                   && (x.Stars == Stars.Length || string.IsNullOrEmpty(Stars))
                                                   && (
                                                         (x.Name_En.ToLower().Contains(HotelName.ToLower()) || string.IsNullOrEmpty(HotelName))
                                                       ||(x.Name_Ar.Contains(HotelName) || string.IsNullOrEmpty(HotelName))
                                                        ))
                                          .ToList();



            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.City = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");

                    hotels = hotels.OrderBy(x => x.Name_En).ToList();

                    break;
                case "ar-SA":
                    ViewBag.City = new SelectList(db.Cities.ToList(), "Id", "Name_Ar");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_Ar");

                    hotels = hotels.OrderBy(x => x.Name_Ar).ToList();
                    break;
                default:
                    ViewBag.City = new SelectList(db.Cities.ToList(), "Id", "Name_En");
                    ViewBag.Country = new SelectList(db.Countries.ToList(), "Id", "Name_En");

                    hotels = hotels.OrderBy(x => x.Name_En).ToList();
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


            return View(hotels.ToPagedList(page ?? 1, 10));
        }

        /*
         * Preview hotel
         */
        [AllowAnonymous]
        public ActionResult HotelPreview(int HotelId)
        {
            Hotel hotel = db.Hotels.Include("City")
                                   .Include("Images")
                                   .Include("HotelRooms")
                                   .Include("HotelRooms.Room")
                                   .Include("HotelRates")
                                   .SingleOrDefault(x => x.Id == HotelId);

            hotel.Rate++;
            db.SaveChanges();

            string userID = User.Identity.GetUserId();

            ViewBag.GuestsCount = new SelectList(db.Rooms.ToList(), "Cust_Count", "Cust_Count");

            int ExRate = hotel.HotelRates.Where(x => x.Rate == "Excellent").Count();
            int GRate = hotel.HotelRates.Where(x => x.Rate == "Good").Count();
            int NBRate = hotel.HotelRates.Where(x => x.Rate == "Not Bad").Count();
            int BRate = hotel.HotelRates.Where(x => x.Rate == "Bad").Count();

            int allRates = hotel.HotelRates.Count;

            float GPer = (float)(GRate * 100) / allRates;
            float ExPer = (float)(ExRate * 100) / allRates;
            float NbPer = (float)(NBRate * 100) / allRates;
            float BPer = (float)(BRate * 100) / allRates;

            ViewBag.GRate = GPer;
            ViewBag.ExRate = ExPer;
            ViewBag.NbRate = NbPer;
            ViewBag.BRate = BPer;

            return View(hotel);
        }

        /*
         * Rate hotel 
         */
        [Authorize(Roles = "User")]
        public PartialViewResult RateHotel(string Rate, int HotelId)
        {
            Hotel hotel = db.Hotels.Include("City")
                                  .Include("Images")
                                  .Include("HotelRooms")
                                  .Include("HotelRooms.Room")
                                  .Include("HotelRates")
                                  .SingleOrDefault(x => x.Id == HotelId);

            string userId = User.Identity.GetUserId();

            HotelRate hr = db.HotelRates.SingleOrDefault(x => x.Hotel.Id == hotel.Id && x.User.Id == userId);

            if (hr == null)
            {
                hr = new HotelRate()
                {
                    Hotel = hotel,
                    User = db.Users.Find(User.Identity.GetUserId()),
                    Rate = Rate
                };
                db.HotelRates.Add(hr);
            }
            else
            {
                hr.Rate = Rate;
            }
            db.SaveChanges();


            int ExRate = hotel.HotelRates.Where(x => x.Rate == "Excellent").Count();
            int GRate = hotel.HotelRates.Where(x => x.Rate == "Good").Count();
            int NBRate = hotel.HotelRates.Where(x => x.Rate == "Not Bad").Count();
            int BRate = hotel.HotelRates.Where(x => x.Rate == "Bad").Count();

            int allRates = hotel.HotelRates.Count;

            float GPer = (float)(GRate * 100) / allRates;
            float ExPer = (float)(ExRate * 100) / allRates;
            float NbPer = (float)(NBRate * 100) / allRates;
            float BPer = (float)(BRate * 100) / allRates;

            ViewBag.GRate = GPer;
            ViewBag.ExRate = ExPer;
            ViewBag.NbRate = NbPer;
            ViewBag.BRate = BPer;

            return PartialView("_HotelRatingPartial", hotel);
        }

        /*
         * Search for room in a hotel 
         */
        [AllowAnonymous]
        public PartialViewResult SearchRoom(int HotelId ,string Type, int? GuestsCount, string DetailsTerm, float? NightPriceGt, float? NightPriceLs)
        {
            List<HotelRoom> hotelRooms = db.HotelRooms.Where(x => (x.Room.Type == Type || Type == "All")
                                                            && (x.Room.Cust_Count == GuestsCount || GuestsCount == null)
                                                            && (x.Room.Details_En.Contains(DetailsTerm)
                                                                || x.Room.Details_Ar.Contains(DetailsTerm)
                                                                || string.IsNullOrEmpty(DetailsTerm))
                                                           
                                                            && x.Hotel.Id == HotelId)
                                                      .ToList();

            if(NightPriceGt != null && NightPriceLs != null)
            {
                hotelRooms = hotelRooms.Where(x => (x.NightPrice > NightPriceGt
                                                   && (x.NightPrice <= NightPriceLs)
                                                   || NightPriceGt == null && NightPriceLs == null))
                                       .ToList();
            }
            else
            {
                hotelRooms = hotelRooms.Where(x => (x.NightPrice > NightPriceGt
                                                   || (x.NightPrice <= NightPriceLs)
                                                   || NightPriceGt == null && NightPriceLs == null))
                                       .ToList();
            }

            Hotel hotel = db.Hotels.Include("City")
                                   .Include("Images")
                                   .Include("HotelRooms")
                                   .Include("HotelRooms.Room")
                                   .SingleOrDefault(x => x.Id == HotelId);

            hotel.HotelRooms = hotelRooms;

            return PartialView("_RoomsTablePartial", hotel);
        }

        /*
         * Add room to hotel
         */
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult AddRoomToHotel(int HotelId)
        {
            Hotel hotel = db.Hotels.Find(HotelId);
            HotelRoom hr = new HotelRoom()
            {
                Hotel = hotel,
                Count = 1,
            };

            ViewBag.Rooms = new SelectList(db.Rooms.ToList(), "Id", "Name");

            return View(hr);
        }


        [Authorize(Roles = "Admin,Hotel Manager")]
        [HttpPost]
        public ActionResult AddRoomToHotel(HotelRoom model)
        {
            try
            {
                Hotel hotel = db.Hotels.Find(model.Hotel.Id);
                HotelRoom hr = new HotelRoom()
                {
                    Hotel = hotel,
                    Count = model.Count,
                    NightPrice = model.NightPrice,
                    Room = db.Rooms.Find(int.Parse(Request["Rooms"].ToString()))
                };
                db.HotelRooms.Add(hr);
                db.SaveChanges();


                return RedirectToAction("HotelRoomsManagement", "Hotel", new { HotelId = hotel.Id });
            }
            catch
            {

            }
            ViewBag.Rooms = new SelectList(db.Rooms.ToList(), "Id", "Name");

            return View(model);
        }

        /*
         * Hotels management page
         */
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult HotelsManagement(int? page,string Search)
        {
            List<Hotel> hotels = new List<Hotel>();
            switch (SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    hotels = db.Hotels.Include("City")
                                                  .Include("City.Country")
                                                  .Include("Images")
                                                  .Include("HotelRooms")
                                                  .Include("HotelRooms.Room")
                                                  .Where(x => x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search))
                                                  .ToList();
                    break;
                case "ar-SA":
                    hotels = db.Hotels.Include("City")
                                                  .Include("City.Country")
                                                  .Include("Images")
                                                  .Include("HotelRooms")
                                                  .Include("HotelRooms.Room")
                                                  .Where(x => x.Name_Ar.Contains(Search) || string.IsNullOrEmpty(Search))
                                                  .ToList();
                    break;
                default:
                    hotels = db.Hotels.Include("City")
                                                  .Include("City.Country")
                                                  .Include("Images")
                                                  .Include("HotelRooms")
                                                  .Include("HotelRooms.Room")
                                                  .Where(x => x.Name_En.Contains(Search) || string.IsNullOrEmpty(Search))
                                                  .ToList();
                    break;

            }

           

            return View(hotels.ToPagedList(page ?? 1, 10));
        }
        /*
         * Hotel's rooms management
         */
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult HotelRoomsManagement(int? page, int HotelId)
        {
            List<HotelRoom> hotelRooms = db.HotelRooms.Include("Hotel")
                                                      .Include("Room")
                                                      .Where(x => x.Hotel.Id == HotelId)
                                                      .ToList();

            switch(SiteLanguages.GetCurrentLanguageCulture())
            {
                case "en-US":
                    ViewBag.HotelName = db.Hotels.Find(HotelId).Name_En;
                    break;
                case "ar-SA":
                    ViewBag.HotelName = db.Hotels.Find(HotelId).Name_Ar;
                    break;
                default:
                    ViewBag.HotelName = db.Hotels.Find(HotelId).Name_En;
                    break;
            }
            ViewBag.HotelId = HotelId;

            return View(hotelRooms.ToPagedList(page ?? 1, 10));
        }

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditHotelRoom(int Id)
        {
            HotelRoom hotelRoom = db.HotelRooms.Include("Hotel")
                                               .Include("Room")
                                               .SingleOrDefault(x => x.Id == Id);
            ViewBag.Rooms = new SelectList(db.Rooms.ToList(), "Id", "Name", hotelRoom.Room.Id);
            return View(hotelRoom);
        }

        /*
         * Edit hotel room
         */
        [HttpPost]
        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult EditHotelRoom(HotelRoom model)
        {
            HotelRoom hotelRoom = db.HotelRooms.Include("Hotel").Include("Room").SingleOrDefault(x => x.Id == model.Id);

            try
            {
                hotelRoom.Count = model.Count;
                hotelRoom.NightPrice = model.NightPrice;
                hotelRoom.Room = db.Rooms.Find(int.Parse(Request["Rooms"].ToString()));
                db.SaveChanges();

                return RedirectToAction("HotelRoomsManagement", "Hotel", new { HotelId = hotelRoom.Hotel.Id });
            }
            catch
            {

            }

            ViewBag.Rooms = new SelectList(db.Rooms.ToList(), "Id", "Name");
            return View(hotelRoom);
        }

        [Authorize(Roles = "Admin,Hotel Manager")]
        public ActionResult RoomsManagement(int? page)
        {
            List<Room> rooms = db.Rooms.ToList();

            return View(rooms.ToPagedList(page ?? 1, 10));
        }
        /*
         * View recommended hotels
         */
        [AllowAnonymous]
        public ActionResult SuggestedHotels(int? page, int CityId, int ResId)
        {
            List<Hotel> hotels = db.Hotels.Include("Images")
                                          .Include("City")
                                          .Include("City.Country")
                                          .Where(x => x.City.Id == CityId)
                                          .OrderByDescending(x => x.Stars)
                                          .ToList();

            ViewBag.ResId = ResId;
            return View(hotels.ToPagedList(page ?? 1, 5));
        }

        /*
         * Book room in hotel
         */
        [Authorize(Roles = "User")]
        public ActionResult Book(int RoomId)
        {
            
            HotelReservation hr = new HotelReservation()
            {
                Room = db.HotelRooms.Include("Room").Include("Hotel").SingleOrDefault(x => x.Id == RoomId)
            };

            return View(hr);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult Book(HotelReservation model, int RoomId)
        {
            HotelRoom room = db.HotelRooms.Include("Hotel")
                                              .Include("Room")
                                              .SingleOrDefault(x => x.Id == RoomId);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            try
            {
                int AvailableRoomsCount = 0;
                DateTime idt;
                DateTime odt;
                try
                {
                    idt = Convert.ToDateTime(model.DisplayCheck_In_Date);
                    odt = Convert.ToDateTime(model.DisplayCheck_Out_Date);
                    if (idt >= odt)
                    {
                        HotelReservation hr1 = new HotelReservation()
                        {
                            Room = room
                        };
                        ViewBag.Alert = "alert alert-danger";
                        ViewBag.Message = Resource.ErrorInvalidDates;
                        return View("Book", hr1);
                    }
                }
                catch
                {
                    HotelReservation hr1 = new HotelReservation()
                    {
                        Room = room
                    };
                    ViewBag.Alert = "alert alert-danger";
                    ViewBag.Message = Resource.ErrorInvalidDates;
                    return View("Book", hr1);
                }
                

                DateTime checkin = new DateTime(idt.Year, idt.Month, idt.Day, 12, 0, 0, 0);
                DateTime checkout = new DateTime(odt.Year, odt.Month, odt.Day, 12, 0, 0, 0);

                try
                {
                    DateTime MaxCheckInDate = db.HotelReservations.Where(x => x.Room.Id == RoomId).Max(x => x.Check_In_Date);

                    DateTime MinCheckOutDate = db.HotelReservations.Where(x => x.Room.Id == RoomId).Min(x => x.Check_Out_Date);

                    AvailableRoomsCount = db.HotelReservations.Where(x => x.Room.Id == RoomId && checkin >= MaxCheckInDate && checkin <= MinCheckOutDate)
                                                                 .Min(x => x.RoomsAvailable);

                }
                catch
                {
                    AvailableRoomsCount = room.Count;
                }

                if (AvailableRoomsCount == 0)
                {
                    HotelReservation hr1 = new HotelReservation()
                     {
                         Room = room
                     };
                    ViewBag.Alert = "alert alert-danger";
                    ViewBag.Message = Resource.NoRoomsAvailable;
                    return View("Book", hr1);
                }
               
                
                HotelReservation res = new HotelReservation()
                {
                    Room = room,
                    Check_In_Date =  checkin,
                    Check_Out_Date = checkout,
                    DisplayCheck_In_Date = checkin.ToString(),
                    DisplayCheck_Out_Date = checkout.ToString(),
                    Guest = db.Users.Find(User.Identity.GetUserId()),
                    ReservationCost = float.Parse(((checkout - checkin).TotalDays * room.NightPrice).ToString()),
                    RoomsAvailable = AvailableRoomsCount - 1,
                    IsBooked = true
                };

                if(res.ReservationCost > db.Users.Find(User.Identity.GetUserId()).Credit)
                {
                    ViewBag.Alert = "alert alert-danger";
                    ViewBag.Message = Resource.NotEnoughCredit;
                    return View("Book", res);
                }

                db.HotelReservations.Add(res);
                db.Users.Find(User.Identity.GetUserId()).Credit -= res.ReservationCost;
                

                db.SaveChanges();

                
                return RedirectToAction("Reservation", "Hotel", new { Id = res.Id });
            }
            catch
            { }

            HotelReservation hr = new HotelReservation()
            {
                Room = room
            };

            return View("Book", hr);
        }
        /*
         * View reservation
         */
        [Authorize(Roles = "User")]
        public ActionResult Reservation(int Id)
        {
            HotelReservation hr = db.HotelReservations.Include("Guest")
                                                      .Include("Room")
                                                      .Include("Room.Room")
                                                      .Include("Room.Hotel")
                                                      .Include("Room.Hotel.City")
                                                      .Include("Room.Hotel.City.Country")
                                                      .SingleOrDefault(x => x.Id == Id);

            return View(hr);
        }

        /*
         * USer's reservations
         */

        [Authorize(Roles = "User")]
        public ActionResult MyHotelReservations(int? page)
        {
             ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
             List<HotelReservation> hrs = db.HotelReservations.Include("Room")
                                                              .Include("Room.Hotel")
                                                              .Where(x => x.Guest.Id == user.Id)
                                                              .ToList();

             return View(hrs.ToPagedList(page ?? 1, 20));
        }

        /*
         * Cancel hotel reservation
         */
        [Authorize(Roles = "User")]
        public ActionResult CancelReservation(int Id)
        {
            HotelReservation hr = db.HotelReservations.Find(Id);
            
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            user.Credit += float.Parse(hr.ReservationCost.ToString());
            

            db.HotelReservations.Remove(hr);
            db.SaveChanges();

            return RedirectToAction("MyHotelReservations", "Hotel");
        }

        /*
         * Change Room Visibility
         */

        [Authorize(Roles = "Admin, Hotel Manager")]
        public PartialViewResult ChangeRoomVisibility(int id)
        {
            Room room = db.Rooms.Find(id);

            if (room.IsAvailable)
            {
                room.IsAvailable = false;
            }
            else
            {
                room.IsAvailable = true;
            }
            db.SaveChanges();

            return PartialView("_ChangeRoomVisibilityPartial", room);
        }

	}
}