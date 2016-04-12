using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hurtownia.DAL;
using Hurtownia.Models;
using Hurtownia.ViewModels;

namespace Hurtownia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "NameCategory");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,ProductDescription,DateAdded,DateEnd,ProductPrice,Quantity,MaxQuantity,CoverFileName,IsBestseller,IsHidden,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "NameCategory", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "NameCategory", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,ProductDescription,DateAdded,DateEnd,ProductPrice,Quantity,MaxQuantity,CoverFileName,IsBestseller,IsHidden,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "NameCategory", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddManyQuality()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        public ActionResult ChangeName()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        public ActionResult ProductsWithDateEndUnder120()
        {
            //var products = db.Products.Include(p => p.Category);
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int time = ts.Days;

                if (time <= 120)
                {
                    productList.Add(item);
                }
            }
            return View(productList.ToList());
        }

        public ActionResult ProductsWithDateEndUnder90()
        {
            //var products = db.Products.Include(p => p.Category);
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int time = ts.Days;

                if (time <= 90)
                {
                    productList.Add(item);
                }
            }
            return View(productList.ToList());
        }

        public ActionResult ProductsWithDateEndUnder60()
        {
            //var products = db.Products.Include(p => p.Category);
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int time = ts.Days;

                if (time <= 60)
                {
                    productList.Add(item);
                }
            }
            return View(productList.ToList());
        }

        public ActionResult ProductsWithDateEnd()
        {
            Statistics stat = new Statistics();
            stat.DateEnd120 = null;
            List<Product> productList120 = new List<Product>();
            List<Product> productList90 = new List<Product>();
            List<Product> productList60 = new List<Product>();

            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int time = ts.Days;

                Product prod = null;
                if (time <= 120 && time > 90)
                {
                    prod = item;
                    //productList.Add(item);
                    //stat.DateEnd120.Add(prod);
                    productList120.Add(item);
                }
                else if (time <= 90 && time > 60)
                {
                    StatisticsHelper statHelper = new StatisticsHelper();
                    statHelper.Name = item.ProductName;
                    statHelper.Price = item.ProductPrice;
                    statHelper.DateEnd = item.DateEnd;
                    //productList.Add(item);
                    //stat.DateEnd90.Add(statHelper);
                    productList90.Add(item);
                }
                else if (time <= 60)
                {
                    StatisticsHelper statHelper = new StatisticsHelper();
                    statHelper.Name = item.ProductName;
                    statHelper.Price = item.ProductPrice;
                    statHelper.DateEnd = item.DateEnd;
                    //productList.Add(item);
                    //stat.DateEnd60.Add(statHelper);
                    productList60.Add(item);
                }
                stat.DateEnd120 = productList120;
                stat.DateEnd90 = productList90;
                stat.DateEnd60 = productList60;
            }


            return View(stat);
        }

        [HttpPost]
        public ActionResult EditionAllProductsQuantity(FormCollection data)
        {
            var val = Request.Form["Quantity"].Split(',').ToList();


            int i = 0;
            foreach (var item in db.Products)
            {
                if (item.ProductId == (i + 1))
                {
                    //item.ProductName = name.ElementAt(i);
                    // item.ProductPrice = Convert.ToDecimal(price.ElementAt(i));
                    item.MaxQuantity += Convert.ToInt32(val.ElementAt(i));
                    i++;
                }
            }

            db.SaveChanges();
            return RedirectToAction("AddManyQuality", "Products");
        }

        [HttpPost]
        public ActionResult AddProductsQuantity(FormCollection data)
        {
            var productsName = Request.Form["item.ProductName"].Split(',').ToList();

            var val = Request.Form["item.Quantity"].Split(',').ToList();


            int i = 0;

            foreach (var name in productsName)
            {
                foreach (var item in db.Products)
                {

                    if (item.ProductName == (String)name)
                    {
                        //item.ProductName = name.ElementAt(i);
                        // item.ProductPrice = Convert.ToDecimal(price.ElementAt(i));
                        item.MaxQuantity += Convert.ToInt32(val.ElementAt(i));
                        //i++;
                    }

                }
                i++;
            }
            db.SaveChanges();
            return RedirectToAction("ProductsDeficit", "Manage");
        }



        public ActionResult ChangePrice()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        [HttpPost]
        public ActionResult ChangePriceForm(FormCollection data)
        {
            var productsName = Request.Form["item.ProductName"].Split(',').ToList();

            var price = Request.Form["item.DateEnd"].Split(',').ToList();


            int i = 0;

            foreach (var name in productsName)
            {
                foreach (var item in db.Products)
                {

                    if (item.ProductName == (String)name)
                    {
                        //item.ProductName = name.ElementAt(i);
                        // item.ProductPrice = Convert.ToDecimal(price.ElementAt(i));
                        //item.MaxQuantity += Convert.ToInt32(val.ElementAt(i));
                        //i++;
                        //item.ProductPrice = Convert.ToDecimal(price.ElementAt(i));

                        item.DateEnd = Convert.ToDateTime(price.ElementAt(i));
                    }

                }
                i++;
            }
            db.SaveChanges();
            return RedirectToAction("ChangePrice", "Products");
        }




        [HttpPost]
        public ActionResult EditionNameAll(FormCollection data)
        {
            var name = Request.Form["Name"].Split(',').ToList();


            int i = 0;
            foreach (var item in db.Products)
            {
                if (item.ProductId == (i + 1))
                {
                    item.ProductName = name.ElementAt(i);
                    i++;
                }
            }

            db.SaveChanges();
            return RedirectToAction("ChangeName", "Products");
        }
        [AllowAnonymous]
        public ActionResult ShowAllProducts(string searchBy, string search)
        {
            var products = db.Products.ToList();

            if (searchBy == "name")
            {
                return View(db.Products.Where(x => x.ProductName.Contains(search) || search == null).
                    OrderByDescending(o => o.ProductName).ToArray());
            }
            else if (searchBy == "category")
            {
                return View(db.Products.Where(x => x.Category.NameCategory.Contains(search) || search == null).
                    OrderByDescending(o => o.Category.NameCategory).ToArray());
            }
            else if (searchBy == "manufacture")
            {
                return View(db.Products.Where(x => x.Manufacture.Name.Contains(search) || search == null).
                    OrderByDescending(o => o.Manufacture.Name).ToArray());
            }
            else if (searchBy == "description")
            {
                return View(db.Products.Where(x => x.ProductDescription.Contains(search) || search == null).
                    OrderByDescending(o => o.ProductDescription).ToArray());
            }

            else
            {
                return View(products);               
            }


        }
        [AllowAnonymous]
        public ActionResult OurPromotions(string searchBy, string search)
        {
            List<Product> productList = new List<Product>();
            foreach (var item in db.Products)
            {
                TimeSpan ts = item.DateEnd - DateTime.Now;
                int between = ts.Days;
                if (between < 120)
                {
                    productList.Add(item);
                }
            }
            if (searchBy == "name")
            {
                return View(productList.Where(x => x.ProductName.Contains(search) || search == null).
                    OrderByDescending(o => o.ProductName).ToArray());
            }
            else if (searchBy == "category")
            {
                return View(productList.Where(x => x.Category.NameCategory.Contains(search) || search == null).
                    OrderByDescending(o => o.Category.NameCategory).ToArray());
            }
            else if (searchBy == "manufacture")
            {
                return View(productList.Where(x => x.Manufacture.Name.Contains(search) || search == null).
                    OrderByDescending(o => o.Manufacture.Name).ToArray());
            }
            else if (searchBy == "description")
            {
                return View(productList.Where(x => x.ProductDescription.Contains(search) || search == null).
                    OrderByDescending(o => o.ProductDescription).ToArray());
            }

            else
            {
                return View(productList);
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
