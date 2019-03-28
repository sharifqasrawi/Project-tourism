using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CityReq")]
        public string Name_En { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CityReq")]
        public string Name_Ar { get; set; }

        [Display(Name = "CityDescription", ResourceType = typeof(Resource))]
        public string Description_En { get; set; }
      
        [Display(Name = "CityDescription", ResourceType = typeof(Resource))]
        public string Description_Ar { get; set; }

        [Display(Name = "CityImages", ResourceType = typeof(Resource))]
        public List<CityImage> Images { get; set; }

        [Display(Name = "CityLocation", ResourceType = typeof(Resource))]
        public string CityLocation { get; set; }


        [Display(Name = "Country", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CountryReq")]

        public Country Country { get; set; }
    }
}