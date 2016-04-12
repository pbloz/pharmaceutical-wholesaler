using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Hurtownia.Infrastructure
{
    //do hangfire i embeded image w wysylanym mailu
    public class Helpers
    {
        //wysyla zadanie webowe pod wskazany adres url
        public static void CallUrl(string serviceUrl)
        {
            // Here we can't use Url.Action - called out of process
            var req = HttpWebRequest.Create(serviceUrl);
            req.GetResponseAsync();
        }
    }
}