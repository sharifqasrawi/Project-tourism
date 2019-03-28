using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailReq")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [Display(Name = "Subject", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "SubjectReq")]
        public string Subject { get; set; }

        [Display(Name = "MessageText", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MessageTextReq")]
        public string Text { get; set; }

        [Display(Name = "Date_Time", ResourceType = typeof(Resource))]
        public DateTime Date_Time { get; set; }

        [Display(Name = "Date_Time", ResourceType = typeof(Resource))]
        public string DisplayDate_Time { get; set; }

        [Display(Name = "IsRead", ResourceType = typeof(Resource))]
        public bool IsRead { get; set; }
    }

    public class ReplyMessageViewModel
    {
        [Display(Name = "Subject", ResourceType = typeof(Resource))]
        public string Subject { get; set; }


        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailReq")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = null, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [Display(Name = "MessageText", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MessageTextReq")]
        public string MessageText { get; set; }
    }

    public class EmailToAllUsers
    {
        [Display(Name = "Subject", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "SubjectReq")]
        public string Subject { get; set; }

        [Display(Name = "EmailText", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailTextReq")]
        public string EmailText { get; set; }
    }
}