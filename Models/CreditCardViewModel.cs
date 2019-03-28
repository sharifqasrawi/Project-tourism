﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelAndTourismWebsite.Resources;

namespace TravelAndTourismWebsite.Models
{
    public class CreditCardViewModel
    {
        public int Id { get; set; }

        [Display(Name = "CardType", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CardTypeReq")]
        public string CardType { get; set; }

        [Display(Name = "NameOnCard", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "NameOnCardReq")]
        public string NameOnCard { get; set; }

        [Display(Name = "CardNumber", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CardNumberReq")]
        public string CardNumber { get; set; }

        [Display(Name = "CardVerificationCode", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CardVerificationCodeReq")]
        public string CardVerificationCode { get; set; }

        [Display(Name = "CardExpirationDate", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "CardExpirationDateReq")]
        public string CardExpirationDate { get; set; }
    }
}