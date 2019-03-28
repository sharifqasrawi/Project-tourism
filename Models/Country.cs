using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Country
    {
        public int Id { get; set; }


        [Display(Name = "Country", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CountryReq")]
        public string Name_En { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CountryReq")]
        public string Name_Ar { get; set; }
       
        [Display(Name = "Flag", ResourceType = typeof(Resource))]
        public string Flag { get; set; }
        public bool Visible { get; set; }
        public List<City> Cities { get; set; }

        public int Rate { get; set; }
    }
}