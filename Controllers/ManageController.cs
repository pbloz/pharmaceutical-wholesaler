using Hurtownia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Data.Entity;
using Hurtownia.DAL;
using System.IO;
using Hurtownia.Infrastructure;
using Hurtownia.ViewModels;
using System.Net;
using Hangfire;

namespace Hurtownia.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        StoreContext db = new StoreContext();

        // private IMailService mailService;

        public ManageController(StoreContext context/*, IMailService mailService*/)
        {
            // this.mailService = mailService;
            this.db = context;
        }
        public ManageController()
        {
            this.db = new StoreContext();
        }


        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            LinkSuccess,
            Error
        }

        private ApplicationUserManager _userManager;
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        // GET: Manage
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            if (TempData["ViewData"] != null)
            {
                ViewData = (ViewDataDictionary)TempData["ViewData"];
            }

            if (User.IsInRole("Admin"))
                ViewBag.UserIsAdmin = true;
            else
                ViewBag.UserIsAdmin = false;

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

            var model = new ManageCredentialsViewModel
            {
                Message = message,
                HasPassword = this.HasPassword(),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1,
                UserData = user.UserData
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile([Bind(Prefix = "UserData")]UserData userData)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.UserData = userData;
                var result = await UserManager.UpdateAsync(user);

                AddErrors(result);
            }
            //jesli sa bledy w uzupelnianiu profilu to nie wyswietla bledu 
            //
            if (!ModelState.IsValid)
            {
                //zapamietuje dane miedzy dwoma kolejnymi requestami
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Prefix = "ChangePasswordViewModel")]ChangePasswordViewModel model)
        {
            // In case we have simple errors - return
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);

            // In case we have login errors
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            var message = ManageMessageId.ChangePasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword([Bind(Prefix = "SetPasswordViewModel")]SetPasswordViewModel model)
        {
            // In case we have simple errors - return
            if (!ModelState.IsValid)
            {
                TempData["ViewData"] = ViewData;
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);

                if (!ModelState.IsValid)
                {
                    TempData["ViewData"] = ViewData;
                    return RedirectToAction("Index");
                }
            }

            var message = ManageMessageId.SetPasswordSuccess;
            return RedirectToAction("Index", new { Message = message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("Index", new { Message = ManageMessageId.LinkSuccess }) : RedirectToAction("Index", new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Index", new { Message = message });
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("password-error", error);
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }


        //public ActionResult OrdersList()
        //{
        //    //spr. czy to admin
        //    bool isAdmin = User.IsInRole("Admin");
        //    ViewBag.UserIsAdmin = isAdmin;

        //    IEnumerable<Order> userOrders;

        //    // For admin users - return all orders
        //    if (isAdmin)
        //    {
        //        userOrders = db.Orders.Include("OrderItems").
        //            OrderByDescending(o => o.DateCreated).ToArray();
        //    }
        //    else
        //    {
        //        var userId = User.Identity.GetUserId();
        //        //ma zwracac wszystkie powiazane z nim zamowienia
        //        userOrders = db.Orders.Where(o => o.UserId == userId).Include("OrderItems").
        //            OrderByDescending(o => o.DateCreated).ToArray();
        //    }

        //    return View(userOrders);
        //}


        public ActionResult OrdersList(string searchBy, string search)
        {
            bool isAdmin = User.IsInRole("Admin");
            ViewBag.UserIsAdmin = isAdmin;
            IEnumerable<Order> userOrders;

            if (isAdmin)
            {

                if (searchBy == "firstName")
                {
                    return View(db.Orders.Where(x => x.FirstName.Equals(search) || search == null).Include("OrderItems").
                        OrderByDescending(o => o.DateCreated).ToArray());
                }
                else if (searchBy == "lastName")
                {
                    return View(db.Orders.Where(x => x.LastName.Equals(search) || search == null).Include("OrderItems").
                        OrderByDescending(o => o.DateCreated).ToArray());
                }
                else if (searchBy == "email")
                {
                    return View(db.Orders.Where(x => x.Email.Equals(search) || search == null).Include("OrderItems").
                        OrderByDescending(o => o.DateCreated).ToArray());
                }

                else
                {
                    userOrders = db.Orders.Include("OrderItems").
                        OrderByDescending(o => o.DateCreated).ToArray();
                }
            }
            else
            {
                var userId = User.Identity.GetUserId();
                //ma zwracac wszystkie powiazane z nim zamowienia
                userOrders = db.Orders.Where(o => o.UserId == userId).Include("OrderItems").
                    OrderByDescending(o => o.DateCreated).ToArray();
            }
            return View(userOrders);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public OrderState ChangeOrderState(Order order)
        {
            //pobirane info o zamowieniu ktore chce zmodyfikowac
            Order orderToModify = db.Orders.Find(order.OrderId);
            orderToModify.OrderState = order.OrderState;
            db.SaveChanges();

            if (orderToModify.OrderState == OrderState.Shipped)
            {
                // Schedule confirmation
                //string url = Url.Action("SendStatusEmail", "Manage", new { orderid = orderToModify.OrderId, lastname = orderToModify.LastName }, Request.Url.Scheme);

                //BackgroundJob.Enqueue(() => Helpers.CallUrl(url));

                //IMailService mailService = new HangFirePostalMailService();
                //mailService.SendOrderShippedEmail(orderToModify);

                // mailService.SendOrderShippedEmail(orderToModify);

                //dynamic email = new Postal.Email("OrderShipped");
                //email.To = orderToModify.Email;
                //email.OrderId = orderToModify.OrderId;
                //email.FullAddress = string.Format("{0} {1}, {2}, {3}", orderToModify.FirstName, orderToModify.LastName, orderToModify.Address, orderToModify.CodeAndCity);
                //email.Send();
            }

            return order.OrderState;
        }

        [AllowAnonymous]
        public ActionResult SendStatusEmail(int orderid, string lastname)
        {
            // This could also be used (but problems when hosted on Azure Websites)
            // if (Request.IsLocal)            

            var orderToModify = db.Orders.Include("OrderItems").Include("OrderItems.Product").SingleOrDefault(o => o.OrderId == orderid && o.LastName == lastname);

            if (orderToModify == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //OrderShippedEmail email = new OrderShippedEmail();
            //email.To = orderToModify.Email;
            //email.OrderId = orderToModify.OrderId;
            //email.FullAddress = string.Format("{0} {1}, {2}, {3}", orderToModify.FirstName, orderToModify.LastName, orderToModify.Address, orderToModify.CodeAndCity);
            //email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [AllowAnonymous]
        public ActionResult SendConfirmationEmail(int orderid, string lastname)
        {
            // orderid and lastname as a basic form of auth

            // Also might be called by scheduler (ie. Azure scheduler), pinging endpoint and using some kind of queue / db

            // This could also be used (but problems when hosted on Azure Websites)
            // if (Request.IsLocal)            

            var order = db.Orders.Include("OrderItems").Include("OrderItems.Product").SingleOrDefault(o => o.OrderId == orderid && o.LastName == lastname);

            if (order == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            OrderConfirmationEmail email = new OrderConfirmationEmail();
            email.To = order.Email;
            email.Cost = order.TotalPrice;
            email.OrderNumber = order.OrderId;
            email.FullAddress = string.Format("{0} {1}, {2}, {3}", order.FirstName, order.LastName, order.Address, order.CodeAndCity);
            email.OrderItems = order.OrderItems;
            email.CoverPath = AppConfig.PhotosFolderRelative;
            email.Send();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [Authorize(Roles = "Admin")]
        //confirmSuccesed do wyswietlania info o sukcesie wykonania operacji
        public ActionResult AddProduct(int? productId, bool? confirmSuccess)
        {

            //edycja
            if (productId.HasValue)
                ViewBag.EditMode = true;
            else
                ViewBag.EditMode = false;

            var result = new EditProductViewModel();
            var categories = db.Categories.ToArray();
            result.Categories = categories;
            var manufactures = db.Manufactures.ToArray();
            result.Manufactures = manufactures;
            result.ConfirmSuccess = confirmSuccess;

            Product a;
            //dodawanie nowego produktu
            if (!productId.HasValue)
            {
                a = new Product();
                var price = a.ProductPrice;
                DateTime d = DateTime.Now;
                if (a.DateEnd < d)
                {
                    ModelState.AddModelError("DateEnd", "Data nie może być przeszła.");

                }
                //if ( price < 0.0m)
                //{
                //    ModelState.AddModelError("ProductPrice", "Wprowadzoną niepoprawną cenę.");

                //}

            }
            else
            {
                //wyszukanie po id do edycji
                a = db.Products.Find(productId);
            }

            result.Product = a;

            return View(result);

        }

        [HttpPost]
        //httpposted dzieki niemu trafi do nas ten plik zahostowany w celu dodania okladki
        public ActionResult AddProduct(HttpPostedFileBase file, EditProductViewModel model)
        {
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            DateTime datenow = DateTime.Now;


            if (model.Product.DateEnd < datenow)
            {
                ModelState.AddModelError("", "Wpisana data jest przeszła.");
                ViewData["data"] = "Niepoprawna data";
                return RedirectToAction("AddProduct", new { confirmSuccess = false });

            }

            if (model.Product.ProductId > 0)
            {
                // Saving existing entry

                db.Entry(model.Product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddProduct", new { confirmSuccess = true });
            }
            else
            {
                // Creating new entry

                var f = Request.Form;
                // Verify that the user selected a file
                if (file != null && file.ContentLength > 0)
                {
                    // Generate filename

                    var fileExt = Path.GetExtension(file.FileName);
                    //unikalne id z rozszerzeniem
                    var filename = Guid.NewGuid() + fileExt;

                    var path = Path.Combine(Server.MapPath(AppConfig.PhotosFolderRelative), filename);
                    file.SaveAs(path);

                    // Save info to DB
                    model.Product.CoverFileName = filename;
                    model.Product.DateAdded = DateTime.Now;
                    if (model.Product.DateEnd < model.Product.DateAdded)
                    {
                        ModelState.AddModelError("", "Niepoprawna data");
                    }
                    //zmiana stanu modelu w celu dopisania go do bazy
                    db.Entry(model.Product).State = EntityState.Added;
                    db.SaveChanges();

                    return RedirectToAction("AddProduct", new { confirmSuccess = true });
                }
                else
                {
                    ModelState.AddModelError("", "Nie wskazano pliku.");
                    var categories = db.Categories.ToArray();
                    model.Categories = categories;
                    return View(model);
                }
            }

        }

        public ActionResult HideProduct(int productId)
        {
            var product = db.Products.Find(productId);
            product.IsHidden = true;
            db.SaveChanges();

            return RedirectToAction("AddProduct", new { confirmSuccess = true });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SalesProductsCount()
        {
            var data = db.Orders.Where(p => p.TotalPrice > 0).OrderByDescending(x => x.TotalPrice).Take(20);

            return View(data.ToList());

        }

        /// <summary>
        /// ////////
        /// </summary>
        /// <returns></returns>

        public ActionResult MostSpentMoneyClients()
        {

            List<MostSpentMoneyModel> richestClients = new List<MostSpentMoneyModel>();
            var clients = db.Orders.GroupBy(c => c.Email).ToList();
            var clu = clients.Distinct();
            int j = 0;
            List<Order> or = new List<Order>();
            foreach (var it in clu)
            {
                or.Add(it.ElementAt(j));
            }
            int i = 0;
            foreach (var item in db.Orders)
            {


                if (richestClients.Any(c => c.Name == item.FirstName))
                {
                    var data = richestClients.First(c => c.Name == item.FirstName);
                    MostSpentMoneyModel msc = new MostSpentMoneyModel();
                    msc.Name = item.FirstName;
                    msc.LastName = item.LastName;

                    msc.Price = data.Price + item.TotalPrice;

                    richestClients.Add(msc);
                    richestClients.Remove(data);

                }
                else if (item.FirstNameAndLastName == item.FirstNameAndLastName)
                {
                    MostSpentMoneyModel msc = new MostSpentMoneyModel();
                    msc.Name = item.FirstName;
                    msc.LastName = item.LastName;
                    msc.Price += item.TotalPrice;
                    richestClients.Add(msc);

                }

            }

            //foreach (var item in richestClients)
            //{
            //    item.Price = item.Price / 2;
            //}
            return View(richestClients.OrderByDescending(p => p.Price).Take(10).ToList());

        }

        public ActionResult MostBuyableClients()
        {
            var data = db.Orders.GroupBy(p => p.FirstName + " " + p.LastName).Select(lista => new CategoryStatistics()
            {
                CategoryName = lista.Key,
                Categorys = lista.Count()
            });

            
            return View(data.OrderByDescending(p => p.Categorys).ToList());
        }
        public ActionResult ProductsDeficit()
        {
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {

                if (item.MaxQuantity < 1000)
                {
                    productList.Add(item);
                }
            }
            return View(productList.ToList());
        }

        public ActionResult ProductsWithDateEnd()
        {
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int between = ts.Days;
                if (between < 90)
                {
                    productList.Add(item);
                }
            }
            return View(productList.ToList());
        }

        public ActionResult MostSellsProducts()
        {
            List<MostBuyableProducts> productList = new List<MostBuyableProducts>();
            foreach (var item in db.OrderItems)
            {
                foreach (var pr in db.Products)
                {
                    if (pr.ProductId == item.ProductId)
                    {
                        if (productList.Any(p => p.Products.ProductId == item.ProductId))
                        {

                            MostBuyableProducts mbp = new MostBuyableProducts();
                            var prod = productList.First(p => p.Products.ProductId == item.ProductId);
                            // mbp.Products = productList.Find(p => p.Products.ProductId == item.ProductId);

                            //mbp.Products = prod.Products;
                            prod.Quantity += item.Quantity;

                            //productList.Add(mbp);
                            //productList.GroupBy(p => p.Products);
                            //productList.Sum(p => p.Quantity);
                        }
                        else
                        {
                            MostBuyableProducts mbp = new MostBuyableProducts();
                            mbp.Products = item.Product;
                            mbp.Quantity = item.Quantity;
                            productList.Add(mbp);

                        }
                    }
                }
            }
            return View(productList.OrderByDescending(p => p.Quantity).Take(10).ToList());
        }

        public ActionResult TheMostProductsInMagazine()
        {

            return View(db.Products.OrderByDescending(p => p.MaxQuantity).Take(10).ToList());
        }

        public ActionResult ProductsWithTheLongestDataEnd()
        {

            return View(db.Products.OrderByDescending(p => p.DateEnd).Take(10).ToList());
        }
        public ActionResult ProductsWithTheShortestDataEnd()
        {

            return View(db.Products.OrderBy(p => p.DateEnd).Take(10).ToList());
        }

        public ActionResult UnhideProduct(int productId)
        {
            var product = db.Products.Find(productId);
            product.IsHidden = false;
            db.SaveChanges();

            return RedirectToAction("AddProduct", new { confirmSuccess = true });
        }
        public ActionResult Widok()
        {
            return View();
        }
    }
}