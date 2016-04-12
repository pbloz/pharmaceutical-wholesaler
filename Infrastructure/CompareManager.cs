using Hurtownia.DAL;
using Hurtownia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Infrastructure
{
    public class CompareManager
    {
        //ref do aktualnego sesisionmanagera
        private ISessionManager session;
        private StoreContext db;
        //stala dla sesji
        public const string CompareSessionKey = "CompareData";

        //isesion
        public CompareManager(ISessionManager session, StoreContext db)
        {
            this.session = session;
            this.db = db;
        }

        public void AddToCompare(int productid)
        {
            //pobiera aktualny stan koszyka z sesji
            var cart = this.GetCompare();
            //sprawdzamy czy ten product co chcemy dodac do koszyka jest w nim czy nie
            var cartItem = cart.Find(c => c.Product.ProductId == productid);

            if (cartItem != null)
            {

            }

            else
            {

                // znajdz produkt i dodaj do karty zamowienia
                var productToAdd = db.Products.Where(a => a.ProductId == productid).SingleOrDefault();
                if (productToAdd != null)
                {
                    //jesli znaleziono 
                    var newCartItem = new CompareItem()
                    {
                        Product = productToAdd

                    };
                    //dodaj do karty
                    cart.Add(newCartItem);
                }
            }

            //zapisujemy ponownie stan listy w sesji
            session.Set(CompareSessionKey, cart);
        }





        public List<CompareItem> GetCompare()
        {
            List<CompareItem> cart;
            //jesli nie ma czegos takiego w koszyu to stworz liste
            if (session.Get<List<CompareItem>>(CompareSessionKey) == null)
            {
                cart = new List<CompareItem>();
            }
            else
            {
                cart = session.Get<List<CompareItem>>(CompareSessionKey) as List<CompareItem>;
                //cart = new List<CompareItem>();

            }

            return cart;
        }

        public int RemoveFromCompare(int productid)
        {
            var cart = this.GetCompare();
            //sprawdzenie czy produkt jest w koszyku
            var cartItem = cart.Find(c => c.Product.ProductId == productid);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }
            session.Set(CompareSessionKey, cart);

            // Return count of removed item currently inside cart
            return 0;
        }


        public void EmptyCart()
        {
            session.Set<List<CompareItem>>(CompareSessionKey, null);
        }

    }
}