using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelAndTourismWebsite.Models
{
    public class RechargeViewModel
    {
       public CreditCardViewModel CreditCard { get; set; }
       public float CurrentCredit { get; set; }
       public float NewCredit { get; set; }
    }
}