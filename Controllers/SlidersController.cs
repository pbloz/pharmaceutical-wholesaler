
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

namespace Hurtownia.Controllers
{
    public class SlidersController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: Sliders
        public ActionResult Index()
        {
            var sliders = db.Sliders.Include(s => s.Product);

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName");
            return View(sliders.ToList());
        }

        // GET: Sliders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // GET: Sliders/Create
        public ActionResult Create()
        {
            var prod = db.Products.ToList();
            var slid = db.Sliders.ToList();
            var result = prod.Where(p => !slid.Any(p2 => p2.ProductId == p.ProductId));

            //var query =
            //            from c in db.Products
            //            where !db.Sliders.Any(o => o.ProductId == c.ProductId)
            //            select c;
            ViewBag.ProductId = new SelectList(result, "ProductId", "ProductName");
            //ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Sliders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SliderId,ProductId")] Slider slider)
        {
            if (ModelState.IsValid)
            {
                var sl = db.Sliders.ToList();
                if (!sl.Any(x => x.ProductId == slider.ProductId))
                {
                    db.Sliders.Add(slider);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Ten produkt jest już w sliderze");
                }

            }
            //var prod = db.Products.ToList();
            //var slid = db.Sliders.ToList();
            //List<Product> list = new List<Product>();
            ////SELECT ALL ELEMENTS FROM DB2.TABLE THAT DO NOT EXISTS ON DB1.TABLE BASED ON EXISTING ID's            
            //var resultFinal = (from e in p
            //                   where !(from m in s
            //                           select m.Product).Contains(e)
            //                   select e
            //                  ).ToList();

            //var result = s.Where(slider => !p.Any)





            return View(slider);
        }

        // GET: Sliders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", slider.ProductId);
            return View(slider);
        }

        // POST: Sliders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SliderId,ProductId")] Slider slider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(slider).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "ProductName", slider.ProductId);
            return View(slider);
        }

        // GET: Sliders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slider slider = db.Sliders.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        // POST: Sliders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slider slider = db.Sliders.Find(id);
            db.Sliders.Remove(slider);
            db.SaveChanges();
            return RedirectToAction("Index");
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
