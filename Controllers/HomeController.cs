using Hurtownia.DAL;
using Hurtownia.Infrastructure;
using Hurtownia.Models;
using Hurtownia.ViewModels;
using NLog;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Hurtownia.Controllers
{
    public class HomeController : Controller
    {
        private StoreContext db = new StoreContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index()
        {
            //Category categoryName = new Category { NameCategory = "Przeciwbólowe", Description = "To kategoria leków przeciwbólowych" };
            //db.Categories.Add(categoryName);
            //db.SaveChanges();

            //var categoriesList = db.Categories.ToList();

            var categories = db.Categories.ToList();

            ICacheProvider cache = new DefaultCacheProvider();

            List<Product> newArrivals;

            //sprawdzanie czy zmienna w cache zostala ustawiona
            //jak nic sie nie zmienilo to bierz z cache
            if (cache.IsSet(Consts.NewItemCacheKey))
            {
                newArrivals = cache.Get(Consts.NewItemCacheKey) as List<Product>;
            }
            else
            {
                newArrivals = db.Products.Where(a => !a.IsHidden && a.MaxQuantity>0).OrderByDescending(a => a.DateAdded).Take(4).ToList();
                cache.Set(Consts.NewItemCacheKey, newArrivals, 30);
            }

            var bestsellers = db.Products.Where(a => !a.IsHidden && a.MaxQuantity > 0 && a.IsBestseller).OrderBy(g => Guid.NewGuid()).Take(4).ToList();

            var slider = db.Sliders.OrderBy(p => p.ProductId).ToList();
            var vm = new HomeViewModel()
            {
                Bestsellers = bestsellers,
                Categories = categories,
                NewArrivals = newArrivals,
                Slider = slider

            };
            return View(vm);
        }

        public ActionResult StaticContent(string viewname)
        {

            return View(viewname);
        }



        public ActionResult About()
        {
            List<Product> productList = db.Products.Where(i => i.ProductId != null).Distinct().ToList();
            ViewData["Products"] = productList;

            return View();
        }

        [HttpPost]
        public ActionResult About(IEnumerable<int> SelectedProducts)
        {
            ViewBag.Message = "Your application description page.";

            //jesli model nie zwraca bledow
            if (ModelState.IsValid)
            {
                List<Product> lista = new List<Product>();
                //foreach (var item in SelectedProducts)
                //{
                //    lista.Add(db.Products.Find(item));   

                //}
                if (SelectedProducts.Count() >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        lista.Add(db.Products.Find(SelectedProducts.ElementAt(i)));
                    }
                }
                else if (SelectedProducts.Count() == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        lista.Add(db.Products.Find(SelectedProducts.ElementAt(i)));
                    }
                }
                else
                {
                    return RedirectToAction("About");
                }

                ViewData["lista"] = (List<Product>)lista;

                return View("Compare");
            }

            List<Product> productList = db.Products.Where(i => i.ProductName != null).Distinct().ToList(); 
            ViewData["Products"] = productList;

            return View();
        }


        public ActionResult OKategoriach()
        {
            var data = db.Products.GroupBy(p => p.Category.NameCategory).Select(lista => new CategoryStatistics()

            {
                CategoryName = lista.Key,
                Categorys = lista.Count()
            });
            //return new PartialViewAsPdf("_OKategoriach", data.ToList());
            return View(data.ToList());

        }

     

        public ActionResult GeneratePDF()
        {
            return new Rotativa.ActionAsPdf("OKategoriach");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string Email, string Topic, string Autor, string Text)
        {


            MailMessage msg = new MailMessage();
            //var temp = Membership.FindUsersByName(User.Identity.Name);
            //temp.

            msg.From = new MailAddress(Email);
            msg.To.Add("hurtexinz@gmail.com");
            msg.Subject = "Kontakt - " + Topic;

            msg.Body = "<span style=" + "color:#3366FF; font-weight: bold> " + Autor + "</span>" + " napisał " + "<br/><br/><br/>" +
                "<span style=" + "color:#3366FF; font-weight: bold>" + Text + "</span><br/>" + "<br/><br/>" +
                "Odpowiedz : " + "<br/><br/>" + Email;
            msg.IsBodyHtml = true;
            SmtpClient sc = new SmtpClient("smtp.gmail.com");
            sc.Port = 25;
            sc.Credentials = new NetworkCredential("hurtexinz@gmail.com", "P@ssw0rd#");
            //sc.EnableSsl = true;
            sc.Send(msg);

            return RedirectToAction("Index", "Home");
        }


        public ActionResult AskAboutProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AskAboutProduct(string EmailReceiver, string Text)
        {


            MailMessage msg = new MailMessage();
            //var temp = Membership.FindUsersByName(User.Identity.Name);
            //temp.

            msg.From = new MailAddress(EmailReceiver);
            msg.To.Add("hurtexinz@gmail.com");
            msg.Subject = "Nieznaleziono produktu ";

            msg.Body = "<span style=" + "color:#3366FF; font-weight: bold> " + EmailReceiver + "</span>" + " nie znalazł  " + "<br/><br/><br/>" +
                "<span style=" + "color:#3366FF; font-weight: bold>" + Text + "</span><br/>" + "<br/><br/>" +
                "Odpowiedz : " + "<br/><br/>" + EmailReceiver;
            msg.IsBodyHtml = true;
            SmtpClient sc = new SmtpClient("smtp.gmail.com");
            sc.Port = 25;
            sc.Credentials = new NetworkCredential("hurtexinz@gmail.com", "P@ssw0rd#");
            //sc.EnableSsl = true;
            sc.Send(msg);

            return RedirectToAction("Index", "Home");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}