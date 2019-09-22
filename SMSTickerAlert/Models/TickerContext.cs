using SMSTickerAlert.Models;
using System.Data.Entity;
using System.Linq;

namespace SMSTickerAlert.Models
{
    public class TickerContext : DbContext
    {
        public TickerContext() : base("TickerConnection")
        {
        }
        public DbSet<Ticker> Tickers { get; set; }
        public DbSet<TickerSMSSettings> TickerSMSSettings { get; set; }
    }
}