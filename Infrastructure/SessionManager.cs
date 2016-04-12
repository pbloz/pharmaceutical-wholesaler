using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Hurtownia.Infrastructure
{
    public class SessionManager:ISessionManager
    {
        //obiekt pozwalajacy na dostanie sie na chwile do sesji oferowanej przez asp.net
        private HttpSessionState session;
        //instancjonuje sesje
        public SessionManager()
        {
            //bedziemy korzystac z kontekstu sesji i do niego sie odwolywac
            
            session = HttpContext.Current.Session;
        }
        //usuwa cala sesje
        public void Abandon()
        {
            session.Abandon();
        }

        public T Get<T>(string key)
        {
            return (T)session[key];
        }

        //dodaje delegat
        //wywolany gdy key nie zostal znaleziony - wart informacji w sejsi jest null - sesja wygasla
        //gdy wygasnie sesja to on okresla co ma byc zwrocone null czy inna wartosc domyslna
        public T Get<T>(string key, Func<T> createDefault)
        {
            T retval;

            if (session[key] != null && session[key].GetType() == typeof(T))
            {
                retval = (T)session[key];
            }
            else
            {
                retval = createDefault();
                session[key] = retval;
            }

            return retval;
        }

        public void Set<T>(string name, T value)
        {
            session[name] = value;
        }
        //opakowuje geta w try catch zeby nie trzeba
        //bylo  za kazdym razem recznie opakowywac
        //jesli nic nie zostanie znalezione to zwroc wartosc domyslna - null
        //brak nullreferenceexception!
        public T TryGet<T>(string key)
        {
            try
            {
                return (T)session[key];
            }
            catch (NullReferenceException)
            {
                return default(T);
            }
        }
    }
}