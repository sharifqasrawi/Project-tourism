using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "FirstName", ResourceType = typeof(Resource))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resource))]
        public string LastName { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(Resource))]
        public string Gender { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resource))]
        public string Email { get; set; }

        [Display(Name = "PhNumber", ResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resource))]
        public string Country { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resource))]
        public string City { get; set; }
        public bool Active { get; set; }

        [Display(Name = "RegistrationDateTime", ResourceType = typeof(Resource))]
        public DateTime? RegistrationDateTime { get; set; }

        [Display(Name = "LastLoginDateTime", ResourceType = typeof(Resource))]
        public DateTime? LastLoginDateTime { get; set; }

        [Display(Name = "LoginsCount", ResourceType = typeof(Resource))]
        public int LoginCount { get; set; }

        [Display(Name = "Credit", ResourceType = typeof(Resource))]
        public float Credit { get; set; }

        [Display(Name = "IsAdmin", ResourceType = typeof(Resource))]
        public bool IsAdmin { get; set; }

        [Display(Name = "PreferredInterfaceLanguage", ResourceType = typeof(Resource))]
        public string PreferredInterfaceLanguage { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<CityImage> CityImages { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightReservation> FlightReservations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<HotelRate> HotelRates { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<HotelReservation> HotelReservations { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferReservation> OfferReservations { get; set; }
        public DbSet<WebsiteRating> WebsiteRatings { get; set; }

     }
}