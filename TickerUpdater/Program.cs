// Copyright 2019 Maria Gabriella Brodi.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using ModelsCommons.Model;
using ModelsCommons.Util;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading;

namespace TickerUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            TwilioConf tw = new TwilioConf()
            {
                Token= ConfigurationManager.AppSettings["SMSKeyToken"],
                From = ConfigurationManager.AppSettings["SMSFrom"],
                Account= ConfigurationManager.AppSettings["SMSAccount"]
            };
            FinProvider fin = new FinProvider()
            {
                Name = ConfigurationManager.AppSettings["FinProviderName"],
                Match = ConfigurationManager.AppSettings["FinProviderMatch"],
                Replace = ConfigurationManager.AppSettings["FinProviderReplace"],
                Url = ConfigurationManager.AppSettings["FinProviderURL"]
            };
            Console.WriteLine("Starting TickerUpdater cycle");
            TickerContext _db = new TickerContext(ConfigurationManager.ConnectionStrings["TickerSQLConnection"].ConnectionString);
            // Initialize the product database.
            Database.SetInitializer(new TickerDatabaseInit());
            while (true)
            {
                try
                {
                    foreach (Ticker ticker in _db.Tickers)
                    {
                        ticker.LastRead = DateTime.UtcNow;
                        var currRead = HTTPhandy.TickerFromAPI(fin, ticker.TickerName);
                        ticker.Current = currRead >0 ? currRead: ticker.Current;
                        Console.WriteLine(ticker.TickerName + ": " + ticker.Current);
                    }
                    _db.SaveChanges();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    var results = from t1 in _db.Tickers
                                  join t2 in _db.TickerSMSSettings on t1.TickerName equals t2.TickerName
                                  where (t1.Current <= t2.Low) || (t1.Current >= t2.High)
                                  select new
                                  {
                                      t1.TickerName,
                                      t1.Current,
                                      t2.Mobile,
                                      t2.Low,
                                      t2.High,
                                      t1.LastRead
                                  };
                    TickerSMSSettings found;
                    foreach (var smsSettings in results.ToList())
                    {
                        Console.WriteLine(smsSettings.TickerName + " " + smsSettings.Mobile + " " + smsSettings.Current);
                        found = _db.TickerSMSSettings.Where(s => s.Mobile == smsSettings.Mobile)
                   .FirstOrDefault<TickerSMSSettings>();
                        found.TickerDate = smsSettings.LastRead;
                        HTTPhandy.SendSMS(tw,found, smsSettings.Current);
                        _db.TickerSMSSettings.Remove(found);
                    }
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(5000);
            }

        }
    }
}
