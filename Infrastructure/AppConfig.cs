using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace Hurtownia.Infrastructure
{
    public class AppConfig
    {
        //helpery, klasy zawierajace stale
        //klasa sluzaca do dostawania do wpisów kongfiguracyjnych z webconfig
        private static string _categoryIconsFolderRelative = ConfigurationManager.AppSettings["CategoryIconsFolder"];
        public static string CategoryIconsFolderRelative
        {
            get
            {
                return _categoryIconsFolderRelative;
            }
        }

        private static string _photosFolderRelative = ConfigurationManager.AppSettings["PhotosFolder"];
        public static string PhotosFolderRelative
        {
            get
            {
                return _photosFolderRelative;
            }
        }
    }
}