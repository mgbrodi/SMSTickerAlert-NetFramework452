using System;
using System.ComponentModel.DataAnnotations;


namespace SMSTickerAlert.Models
{
    public class Ticker
    {

        [Key, StringLength(100), Display(Name = "Name")]
        public string TickerName { get; set; }

        [Display(Name = "Short Description")]
        public string Description { get; set; }


        [Display(Name = "Current Value")]
        public decimal Current { get; set; }


        [Display(Name = "Last Read on")]
        public DateTime LastRead { get; set; }
    }
}