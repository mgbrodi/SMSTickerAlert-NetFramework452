using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TickerUpdater.Models;
using MessageBird;

namespace TickerUpdater.Util
{
    public static class HTTPhandy
    {
        public static void SendSMS(TickerSMSSettings ticker)
        {
            //StringBuilder postData = new StringBuilder("");
            //curl "https://platform.clickatell.com/messages/http/send?apiKey=gKDqcnrjTPe4sNiB88_pAA==&to=19143343722&content=Test+message+text"
            // curl - X POST https://rest.messagebird.com/messages -H 'Authorization: AccessKey HIDDEN_API_KEY' 
            // -d "recipients=TO"
            // -d "originator=FROM"
            // -d "body=BODY"

           
            Client client = Client.CreateDefault(Properties.Settings.Default.SMSKeyValue);
            long Msisdn;
            Int64.TryParse(ticker.Mobile,out Msisdn);

            MessageBird.Objects.Message message =
            client.SendMessage("MessageBird", ticker.TickerName+" - "+ticker.Current+" - Tresholds high: "+ticker.High+" low: "+ticker.Low, new[] { Msisdn });
            
            //// Create a request using a URL that can receive a post.   
            //WebRequest request = WebRequest.Create(Properties.Settings.Default.SMSUrl);
            //// Set the Method property of the request to POST.  
            //request.Method = "POST";

            //// Create POST data and convert it to a byte array.  
            ////postData.AppendFormat(@"{""content"":""Ticker: {0}, current: {1}, high: {2}, low: {3}"", ""to"":[""{4}""]}",
            ////    ticker.TickerName, ticker.Current.ToString(), ticker.High.ToString(), ticker.Low.ToString(), "19143343722");

            //postData.Append(@"{""content"":""Ticker: 0, current: , high: 1 , low: 2"", ""to"":[""19143343722""]}");
            //// ticker.TickerName);//, ticker.Current.ToString(), ticker.High.ToString(), ticker.Low.ToString(), "19143343722");


            //byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());

            //// Set the ContentType property of the WebRequest.  
            //request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Bearer " + Properties.Settings.Default.SMSKeyValue);

            //// Set the ContentLength property of the WebRequest.  
            //request.ContentLength = byteArray.Length;

            //// Get the request stream.  
            //Stream dataStream = request.GetRequestStream();
            //// Write the data to the request stream.  
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //// Close the Stream object.  
            //dataStream.Close();

            //// Get the response.  
            //WebResponse response = request.GetResponse();

            //// Close the response.  
            //response.Close();

            //ticker.Mobile = "";
        }


        public static Decimal TickerFromAPI(string name)
        {
            Decimal current = 0;
            if (name != null && name.Length > 0)
            {
                var url = TickerUpdater.Properties.Settings.Default.FinProviderURL.Replace(
                    TickerUpdater.Properties.Settings.Default.FinProviderReplace, name);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                WebResponse response = request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();

                    //marketwatch <==> yahoo
                    if (TickerUpdater.Properties.Settings.Default.FinProviderName == "marketwatch")
                    {
                        //<meta name="price" content="220.83">
                        Match match = Regex.Match(responseFromServer, @"<meta name=""price"" content=""(\d+,?\d+\.\d+)"">",
                            RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            Decimal.TryParse(match.Groups[1].Value, out current);
                        }
                    }
                }

                response.Close();
            }
            return current;
        }
    }
}