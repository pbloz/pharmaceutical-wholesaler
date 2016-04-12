using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hurtownia.Infrastructure
{
    // statyczna aby mozna bylo zdefiniowac metody rozszerzeniowe
    public static class UrlHelpers
    {
        //metoda roszerzeniowa musi byc statyczna
        // this infformuje do ktorej klasy ma byc ta metoda dopisana
        //combine skleja folder z nazwa pliku
        //content generuje sciezke
        public static string CategoryIconPath(this UrlHelper helper, string categoryIconFilename)
        {
            var categoryIconFolder = AppConfig.CategoryIconsFolderRelative;
            var path = Path.Combine(categoryIconFolder, categoryIconFilename);
            var absolutePath = helper.Content(path);
            return absolutePath;
        }


        public static string ProductCoverPath(this UrlHelper helper, string productFilename)
        {
            var productCoverFolder = AppConfig.PhotosFolderRelative;
            var path = Path.Combine(productCoverFolder, productFilename);
            var absolutePath = helper.Content(path);
            return absolutePath;
        }
    }
}