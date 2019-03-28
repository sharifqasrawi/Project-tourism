using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        public string Name { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "TypeReq")]
        public string Type { get; set; }

        [Display(Name = "Cust_Count", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Cust_CountReq")]
        public int Cust_Count { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_En { get; set; }

        [Display(Name = "Details", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "DetailsReq")]
        public string Details_Ar { get; set; }

         [Display(Name = "Status", ResourceType = typeof(Resource))]
        public bool IsAvailable { get; set; }
    }
}