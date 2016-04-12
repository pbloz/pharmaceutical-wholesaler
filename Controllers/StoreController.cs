using Hurtownia.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;
using Hurtownia.Models;
using Hurtownia.Infrastructure;
using System.Web.Security;

namespace Hurtownia.Controllers
{
    public class StoreController : Controller
    {
        StoreContext db = new StoreContext();
        private ShoppingCartManager shoppingCartManager;
        private CompareManager compareManager;

        private ApplicationUserManager _userManager;
        public ISessionManager sessionManager { get; set; }

        public ISessionManager sessionCompare { get; set; }

        public StoreController()
        {
            this.sessionManager = new SessionManager();
            this.sessionCompare = new SessionManager();
            this.shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
            this.compareManager = new CompareManager(this.sessionCompare, this.db);
        }
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);
            return View(product);
        }

        //public ActionResult Refer(int id)
        //{
        //    var product = db.Products.Find(id);
        //    return View(product);
        //}



        [HttpPost]
        public ActionResult Refer(int id, string EmailReceiver, string Text)
        {

            var product = db.Products.Find(id);
            String uth = User.Identity.Name;
            var temp = db.Users.First(u => u.Email == uth);
            String Autor = temp.UserData.FirstNameAndLastName;
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("karwowski.pawel992@gmail.com");
            msg.To.Add(EmailReceiver);
            msg.Subject = "Okazja, nie przegap!;)  ";
            // msg.From = new MailAddress("hurtexinz@gmail.com");
            msg.Body = "<span style=" + "color:#3366FF; font-weight: bold> " + "Witaj !" + "</span><br/>" +
                 "Twoj znajomy " + "<span style=" + "color:#3366FF; font-weight: bold>" + Autor + "</span><br/>" + " polecił Ci  produkt " +
                 " <a href=" + "https://localhost:44300/product-" + id + ".html" + " style=" + "color:#3366FF; font-weight: bold>" +
                 "<strong> " + product.ProductName + "</strong><br/></a>" + " W załączniku dopisał : " + " <br/><br/>" + Text + " <br/>" +
                 "<br/>" + " Pozdrawiamy" + "<br/>" + "<a href=" + "https://localhost:44300/" + ">" + "AppFarm" + "</a>" +
                 " Sp. z o.o, " + "<br/>" + "ul. 3 Maja 54, " + "<br/>" + "08-100 Siedlce !";

            msg.IsBodyHtml = true;
            SmtpClient sc = new SmtpClient("smtp.gmail.com");
            sc.Port = 25;
            sc.Credentials = new NetworkCredential("hurtexinz@gmail.com", "P@ssw0rd#");
            //sc.EnableSsl = true;
            sc.Send(msg);

            return RedirectToAction("Details", "Store");
        }




        public ActionResult List(string categoryname, string sortOrder, string currentFilter, int? page, string searchQuery = null)
        {
            //pobiranie kategorii i produktow powiazanych z nim
            //category zwraca dokładnie jedna kategorie bo jest single na koncu
            // var category = db.Categories.Include("Products").Where(c => c.NameCategory.ToUpper() == categoryname.ToUpper()).Single();
            //porownojemy categoryname z tym co jest w bazie
            var category = db.Categories.Include("Products").Where(g => g.NameCategory.ToUpper() == categoryname.ToUpper()).Single();
            //lista produktow z danej kategorii
            //

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchQuery != null)
            {

                page = 1;
            }
            else
            {
                searchQuery = currentFilter;
            }

            ViewBag.CurrentFilter = searchQuery;

            var products = category.Products.Where(p => (searchQuery == null ||
                                                p.ProductName.ToLower().Contains(searchQuery.ToLower()) ||
                                                p.ProductDescription.ToLower().Contains(searchQuery.ToLower())) &&
                                                !p.IsHidden);

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.ProductName);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.ProductDescription);
                    break;

                default:
                    products = products.OrderBy(s => s.ProductPrice);
                    break;
            }
            //sprawdzanie czy wyslane żądanie jest wyslanie przez ajax czy nie
            if (Request.IsAjaxRequest())
            {
                return PartialView("ProductList", products);
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            //return View(products.ToList());

            return View(products.ToPagedList(pageNumber, pageSize));
        }



        [HttpPost]
        public ActionResult List(IEnumerable<int> SelectedProducts)
        {

            //jesli model nie zwraca bledow
            if (ModelState.IsValid)
            {
                List<Product> lista = new List<Product>();
                //foreach (var item in SelectedProducts)
                //{
                //    lista.Add(db.Products.Find(item));   //dodawanie studentow o peselu przechwyconym za pomoca selected

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
                    return RedirectToAction("List");
                }

                ViewData["lista"] = (List<Product>)lista;

                //return View("Compare");
                return View("Compare");
            }

            List<Product> productList = db.Products.Where(i => i.ProductName != null).Distinct().ToList(); //do widoku idzie selected list z tego pobiera pesel studenta
            ViewData["Products"] = productList;//(List<Student>)db.Studenci.ToList();

            return View();
        }






        //mozemy sie do niej  odwolac wpisujac jedynie /store/CategoriesMenu
        //mozliwa do wywolania z poziomu podrzadania generowanego przez htmlaction w jakims widoku
        [ChildActionOnly]
        //informuje ze wynik zwracanej akcji
        //jesli w location damy client to wynik zostanie zwrocony w przegladarce 
        //i nie bedzie musiala wysylac zapytania ponownie tylko wezmie z zasobu
        //domyslnie jest ---------any
        //duration okresla czas przechowywania odpowiedzi
        [OutputCache(Duration = 80000)]
        public ActionResult CategoriesMenu()
        {
            var categories = db.Categories.ToList();
            return PartialView(categories);

        }
        //term to co wysyla user w przegladarce podczas wyszukiwania
        public ActionResult ProductsSuggestions(string term)
        {
            //jqueryiu
            //poniewwaz json nie obsluguje calego jednego produktu z wszystkimi o nim danymi to
            //trzeba stworzyc nowy obiekt label
            //nowa klasa zwracajaca label
            //select - projekcja tworzenie nowego 
            //zwracamy jsona - helper


            var products = this.db.Products.Where(p => p.ProductName.ToLower().
                Contains(term.ToLower()) && !p.IsHidden).Take(5).
                    Select(p => new { label = p.ProductName });
            //drugi arg - brak bledow przy metodzie get
            return Json(products, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult Comment(int id, string Text)
        {

            Product product = db.Products.FirstOrDefault(p => p.ProductId == id);
            Comment comment = new Comment();
            //var dat = System.Security.Principal.WindowsIdentity.GetCurrent().User;
            comment.UserName = User.Identity.Name;
            comment.Description = Text;
            comment.CommentId = 1;
            comment.ProductComId = id;
           //var temp=  db.Products.Where(p => p.ProductPrice < 100 && p.ProductPrice > 500);
           //foreach (var item in temp)
           //{
           //    Console.WriteLine(item.ProductName);
           //}
            //product.Comments.Add(comment);
            if (product != null)
            {
                //product.Comments.Add(comment);
                    if (product.ProductId == id)
                    {
                        //shoppingCartManager.Comment(id, Text);
                        //product.Comments.Add(comment);
                        if (product.Comments == null)
                        {
                            product.Comments = new List<Comment>();
                            product.Comments.Add(comment);
                            
                            db.Products.FirstOrDefault(p => p.ProductId == id).Comments.Add(comment);

                            db.SaveChangesAsync();
                        }
                        else {
                            db.Products.FirstOrDefault(p => p.ProductId == id).Comments.Add(comment);
                            db.SaveChanges();
                        }


                    }
                
            }


            return RedirectToAction("Details", "Store");
        }
    }
}