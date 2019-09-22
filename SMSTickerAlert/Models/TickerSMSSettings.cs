using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMSTickerAlert.Models
{
    public class TickerSMSSettings
    {
        //SMS  to
        [Key]
        public string Mobile { get; set; }

        //wich ticker
        public string TickerName { get; set; }

        // retieved on
        public DateTime TickerDate { get; set; }

        //trigger on variation up
        public Decimal High { get; set; }

        //trigger on variation down
        public Decimal Low { get; set; }

    }
}