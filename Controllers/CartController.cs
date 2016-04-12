using Hurtownia.DAL;
using Hurtownia.Models;
using Hurtownia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Hurtownia.Infrastructure;
using System.Web.Hosting;
using System.Net;
using System.Net.Mail;

using Hangfire;
using NLog;
using Rotativa;

namespace Hurtownia.Controllers
{
    public class CartController : Controller
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        private ShoppingCartManager shoppingCartManager;
        private CompareManager compareManager;

        private ApplicationUserManager _userManager;
        public ISessionManager sessionManager { get; set; }

        public ISessionManager sessionCompare { get; set; }

        private StoreContext db = new StoreContext();

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public CartController()
        {
            this.sessionManager = new SessionManager();
            this.sessionCompare = new SessionManager();
            this.shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
            this.compareManager = new CompareManager(this.sessionCompare, this.db);
        }
        // GET: Cart
        public ActionResult Index()
        {
            //sesja i kontekst jako param.
            ShoppingCartManager shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
            //CompareManager compareManager = new CompareManager(this.sessionManager, this.db);
            //zwraca liste cartItem ktora w tym momencie jest zapisana w sesji
            //pobieramy stan koszyka
            var cartItems = shoppingCartManager.GetCart();

            //pobieramy laczna cene
            var cartTotalPrice = shoppingCartManager.GetCartTotalPrice();
            //ustawiany na to co pobralismy
            CartViewModel cartVM = new CartViewModel() { CartItems = cartItems, TotalPrice = cartTotalPrice };
            return View(cartVM);
        }
        //id ident produktu który chcemy dodac
        public ActionResult AddToCart(int id)
        {
            shoppingCartManager.AddToCart(id);
            //gdy uzytokownik wywola akcje addtocart do trafi do akcji index
            return RedirectToAction("Index");
        }

        public ActionResult Compare1()
        {
            //sesja i kontekst jako param.
            CompareManager compareManager = new CompareManager(this.sessionCompare, this.db);
            //zwraca liste cartItem ktora w tym momencie jest zapisana w sesji
            //pobieramy stan koszyka
            var compareItems = compareManager.GetCompare();

            //ustawiany na to co pobralismy
            CompareViewModel compareVM = new CompareViewModel() { CompareItems = compareItems };
            return View("Compare", compareVM);
            //return Json(compareVM,JsonRequestBehavior.AllowGet);
            // return PartialView("_Logo",compareVM);

        }
        public ActionResult Compare()
        {
            //sesja i kontekst jako param.
            CompareManager compareManager = new CompareManager(this.sessionCompare, this.db);
            //zwraca liste cartItem ktora w tym momencie jest zapisana w sesji
            //pobieramy stan koszyka
            var compareItems = compareManager.GetCompare();

            //ustawiany na to co pobralismy
            CompareViewModel compareVM = new CompareViewModel() { CompareItems = compareItems };
            return View(compareVM);
            // return Json(compareVM);

        }



        public ActionResult AddToCompare(int id)
        {
            compareManager.AddToCompare(id);


            return RedirectToAction("Compare");


        }
        //testowe
        public ActionResult AddToCompare1(int productId)
        {
            compareManager.AddToCompare(productId);


            return RedirectToAction("Compare1");


        }

        public ActionResult AddToCart2(int id, int Quantity)
        {
            Product product = db.Products.First(p => p.ProductId == id);
            if (product.MaxQuantity < Quantity)
            {
                ModelState.AddModelError("", "Nie mamy takiej ilości danego produktu w magazynie.");
                return RedirectToAction("Details", "Store", id);
            }
            else if (Quantity < 0)
            {

                ModelState.AddModelError("", "Wprowadzono nieodpowiednią liczbę.");
                return RedirectToAction("Details", "Store", id);
            }
            else
            {
                shoppingCartManager.AddToCart2(id, Quantity);
                //gdy uzytokownik wywola akcje addtocart do trafi do akcji index
                return RedirectToAction("Index");
            }
        }



        //zwraca ilosc w koszyku
        public int GetCartItemsCount()
        {
            return shoppingCartManager.GetCartItemsCount();
        }

        public ActionResult RemoveFromCart(int productId)
        {
            ShoppingCartManager shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
            Product p = db.Products.Find(productId);
            if (p.MaxQuantity > 0)
            {
                int itemCount = shoppingCartManager.RemoveFromCart(productId);
                //ilosc elem aktualnie w koszyku
                int cartItemsCount = shoppingCartManager.GetCartItemsCount();
                decimal cartTotal = shoppingCartManager.GetCartTotalPrice();
                db.SaveChanges();
                // Return JSON to process it in JavaScript
                //struktura 
                //ktora ma zostac zwrocona po wcisniecie remove
                var result = new CartRemoveViewModel
                {
                    RemoveItemId = productId,
                    RemovedItemCount = itemCount,
                    CartTotal = cartTotal,
                    CartItemsCount = cartItemsCount
                };

                return Json(result);
            }
            return View();
        }

        public ActionResult RemoveFromCompare(int productId)
        {

            CompareManager compareManager = new CompareManager(this.sessionCompare, this.db);
            int itemCount = compareManager.RemoveFromCompare(productId);
            db.SaveChanges();

            var compareItems = compareManager.GetCompare();

            //ilosc elem aktualnie w koszyku

            // Return JSON to process it in JavaScript
            //struktura 
            //ktora ma zostac zwrocona po wcisniecie remove
            //var result = new CompareRemoveViewModel
            //{
            //    RemoveItemId = productId,
            //    RemovedItemCount = itemCount,
            //};


            CompareViewModel compareVM = new CompareViewModel() { CompareItems = compareItems };

            return Json(compareVM);
            //return Json(result, JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> Checkout()
        {
            if (Request.IsAuthenticated)
            {
                //pobranie info o aktualnie zalogowanym uzytkowniku
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                //obiekt zamowienia i przepisanie danych o userze
                var order = new Order
                {
                    //pobranie danych o userze
                    FirstName = user.UserData.FirstName,
                    LastName = user.UserData.LastName,
                    Address = user.UserData.Address,
                    CodeAndCity = user.UserData.CodeAndCity,
                    Email = user.UserData.Email,
                    PhoneNumber = user.UserData.PhoneNumber,
                    Factory = user.UserData.Factory,
                    Nip = user.UserData.Nip
                };

                return View(order);
            }
            //jesli nie jest zalogowany to przekieruj do logowania 
            //po zalogowaniu przenies na strone checkout
            else
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout", "Cart") });
        }

        public ActionResult GeneratePDF()
        {
            return new Rotativa.ActionAsPdf("Checkout");
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(Order orderdetails)
        {
            if (ModelState.IsValid)
            {
                // logger.Info("Checking out");

                // Get user
                var userId = User.Identity.GetUserId();

                // Save Order
                ShoppingCartManager shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);
                //pobierze dane zamowienia i id usera i powiaze je ze soba
                var newOrder = shoppingCartManager.CreateOrder(orderdetails, userId);

                // Update profile information
                var user = await UserManager.FindByIdAsync(userId);

                TryUpdateModel(user.UserData);
                await UserManager.UpdateAsync(user);

                // Empty cart
                shoppingCartManager.EmptyCart();

                // Send mail confirmation
                // Refetch - need also products details
                //var order = db.Orders.Include("OrderItems").SingleOrDefault(o => o.OrderId == newOrder.OrderId);   

                //zwroci wypelnione wlasciwowsci nawigacyjne 
                //dot. produktu i zamowienia 
                var order = db.Orders.Include("OrderItems").Include("OrderItems.Product").SingleOrDefault(o => o.OrderId == newOrder.OrderId);


                //imailservice mailservice = new hangfirepostalmailservice();
                //mailservice.sendorderconfirmationemail(order);

                // this.mailservice.sendorderconfirmationemail(order);

                string url = Url.Action("SendConfirmationEmail", "Manage", new { orderid = newOrder.OrderId, lastname = newOrder.LastName }, Request.Url.Scheme);

                //// hangfire - nice one (if asp.net app will be still running)
                //BackgroundJob.Enqueue(() => Helpers.CallUrl(url));



                //// Strongly typed - without background
                //ze strony postal
                OrderConfirmationEmail email = new OrderConfirmationEmail();
                email.To = order.Email;
                email.Cost = order.TotalPrice;
                email.OrderNumber = order.OrderId;
                email.FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity);
                email.OrderItems = order.OrderItems;
                email.CoverPath = AppConfig.PhotosFolderRelative;
                email.Send();

                return RedirectToAction("OrderConfirmation");
                //return new PartialViewAsPdf("_Checkout", order);
            }
            else
                return View(orderdetails);
        }

        public ActionResult OrderConfirmation()
        {
            return View();
        }

        public ActionResult RabatCode(String Code)
        {
            foreach (var item in db.BonusCodes)
            {
                if (item.Code == Code)
                {
                    ShoppingCartManager shoppingCartManager = new ShoppingCartManager(this.sessionManager, this.db);

                    //var price = 0.9m *shoppingCartManager.GetCartTotalPrice();


                    var cartItems =shoppingCartManager.GetCart();

                    //pobieramy laczna cene
                    var cartTotalPrice = 0.9m * shoppingCartManager.GetCartTotalPrice();
                    //ustawiany na to co pobralismy
                    CartViewModel cartVM = new CartViewModel() { CartItems = cartItems, TotalPrice = cartTotalPrice };

                    BonusCode bonusCode = db.BonusCodes.First(x => x.Code == Code);
                    db.BonusCodes.Remove(bonusCode);
                    //db.SaveChanges();
                    this.db.SaveChangesAsync();
                    ViewBag.Code = true;
                    return View("Index",cartVM);

                }

            }
            return RedirectToAction("Index", "Cart");
        }
    }
}