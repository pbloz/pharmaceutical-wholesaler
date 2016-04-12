using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Hurtownia.Infrastructure
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private Cache Cache { get { return HttpContext.Current.Cache; } }
        public object Get(string key)
        {
            return Cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            //czas cachowania
            var expirationTime = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            //4 i 5 param ze mozna cachowac np. przez 10 min jesli nic sie nie zrobi user to dane sa kasowane
            Cache.Insert(key, data, null, expirationTime, Cache.NoSlidingExpiration);
        }

        //jesli null cache nie jest ustawiona
        public bool IsSet(string key)
        {
            return (Cache[key] != null);
        }

        public void Invalidate(string key)
        {
            Cache.Remove(key);
        }
    }
}