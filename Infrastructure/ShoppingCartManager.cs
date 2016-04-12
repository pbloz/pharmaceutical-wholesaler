using Hurtownia.DAL;
using Hurtownia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Infrastructure
{
    public class ShoppingCartManager
    {
        //ref do aktualnego sesisionmanagera
        private ISessionManager session;
        private StoreContext db;
        //stala dla sesji
        public const string CartSessionKey = "CartData";

        //isesion
        public ShoppingCartManager(ISessionManager session, StoreContext db)
        {
            this.session = session;
            this.db = db;
        }

        public void AddToCart(int productid)
        {
            //pobiera aktualny stan koszyka z sesji
            var cart = this.GetCart();
            //sprawdzamy czy ten product co chcemy dodac do koszyka jest w nim czy nie
            var cartItem = cart.Find(c => c.Product.ProductId == productid);
            //dodawannie pojedynczego produktu
            if (cartItem != null)
            {
                if (cartItem.Product.MaxQuantity != 0)
                {
                    cartItem.Quantity++;
                    cartItem.Product.MaxQuantity--;
                }
            }
            else
            {
                // znajdz produkt i dodaj do karty zamowienia
                var productToAdd = db.Products.Where(a => a.ProductId == productid).SingleOrDefault();
                if (productToAdd != null)
                {
                    if (productToAdd.MaxQuantity != 0)
                    {
                        //zmniejsz bo dodano do koszyka 1 sztuke
                        productToAdd.MaxQuantity--;
                        //jesli znaleziono 
                        var newCartItem = new CartItem()
                            {
                                Product = productToAdd,
                                Quantity = 1,
                                TotalPrice = productToAdd.ProductPrice
                            };
                        //dodaj do karty
                        cart.Add(newCartItem);
                    }
                }
            }
            //zapisujemy ponownie stan listy w sesji
            session.Set(CartSessionKey, cart);
        }



        public void AddToCart2(int productid, int Quantity)
        {
            //pobiera aktualny stan koszyka z sesji
            var cart = this.GetCart();

            var cartItem = cart.Find(c => c.Product.ProductId == productid);
            // znajdz produkt i dodaj do karty zamowienia
            var productToAdd1 = db.Products.Where(a => a.ProductId == productid).SingleOrDefault();

            if (cartItem != null)
            {
                if (Quantity > cartItem.Product.MaxQuantity)
                {
                    cartItem.Quantity += cartItem.Product.MaxQuantity;
                    productToAdd1.MaxQuantity = 0;
                    cartItem.Product.MaxQuantity = 0;
                }
                else
                {
                    cartItem.Quantity += Quantity;
                    productToAdd1.MaxQuantity -= Quantity; 
                    cartItem.Product.MaxQuantity -= Quantity;
                }
                db.SaveChangesAsync();
            }
            else
            {
                // znajdz produkt i dodaj do karty zamowienia
                var productToAdd = db.Products.Where(a => a.ProductId == productid).SingleOrDefault();

                if (productToAdd != null)
                {
                    //jesli chce wiecej niz jest dostepne
                    if (Quantity > productToAdd.MaxQuantity)
                    {
                        Quantity = productToAdd.MaxQuantity;
                        productToAdd.MaxQuantity = 0;
                    }
                    else
                    {
                        productToAdd.MaxQuantity -= Quantity;
                    }
                   
                    //jesli znaleziono 
                    var newCartItem = new CartItem()
                    {
                        Product = productToAdd,
                        Quantity = Quantity,
                        TotalPrice = productToAdd.ProductPrice,
                    };
                    //dodaj do karty
                    cart.Add(newCartItem);
                    db.SaveChangesAsync();
                }
            }

            //zapisujemy ponownie stan listy w sesji
            session.Set(CartSessionKey, cart);
        }

        public List<CartItem> GetCart()
        {
            List<CartItem> cart;
            //jesli nie ma czegos takiego w koszyu to stworz liste
            if (session.Get<List<CartItem>>(CartSessionKey) == null)
            {
                cart = new List<CartItem>();
            }
            else
            {
                cart = session.Get<List<CartItem>>(CartSessionKey) as List<CartItem>;
            }

            return cart;
        }

        public int RemoveFromCart(int productid)
        {
            var cart = this.GetCart();
            //sprawdzenie czy produkt jest w koszyku
            var cartItem = cart.Find(c => c.Product.ProductId == productid);

            if (cartItem != null)
            {

                var productToAddQuantity = db.Products.Where(a => a.ProductId == productid).SingleOrDefault();
                //jesli wiecej niz jeden to zmniejsz ilosc o jeden i zwroc ilosc
                if (cartItem.Quantity <= 5)
                {
                    cartItem.Quantity--;
                    productToAddQuantity.MaxQuantity++;
                    return cartItem.Quantity;
                }
                if (cartItem.Quantity < 0)
                {

                    return cartItem.Quantity = 0;
                }
                //jesli jest wiecej niz 5 egzemplarzy to usun calkowicie z koszyka
                else
                {
                    productToAddQuantity.MaxQuantity += cartItem.Quantity;
                    cart.Remove(cartItem);
                }
            }
            db.SaveChanges();
            session.Set(CartSessionKey, cart);

            // Return count of removed item currently inside cart
            return 0;
        }
        public decimal GetCartTotalPrice()
        {
            //iteruje po wszystkich elem koszyka
            var cart = this.GetCart();
            //zwraca sume do zaplacenia
            return cart.Sum(c => (c.Quantity * c.Product.ProductPrice));
        }
        //ilosc elementow mamy w koszyku
        //tam na gorze strony
        public int GetCartItemsCount()
        {
            var cart = this.GetCart();
            int count = cart.Sum(c => c.Quantity);

            return count;
        }

        //order - bo zawiera wypelnione info o userze
        public Order CreateOrder(Order newOrder, string userId)
        {
            var cart = this.GetCart();

            newOrder.DateCreated = DateTime.Now;
            newOrder.UserId = userId;

            //dodanie zamoweinai do tabeli orders
            this.db.Orders.Add(newOrder);

            if (newOrder.OrderItems == null)
                newOrder.OrderItems = new List<OrderItem>();

            decimal cartTotal = 0;
            //dla kazdefgo produktu w koszyku pobierz ingo o tym co to i ile tego jest
            foreach (var cartItem in cart)
            {
                var newOrderItem = new OrderItem()
                {
                    ProductId = cartItem.Product.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.ProductPrice
                };
                //sumowanie lacznej wartosci zamowienia
                cartTotal += (cartItem.Quantity * cartItem.Product.ProductPrice);
                //do orferitems dodajemy nowy wpis
                newOrder.OrderItems.Add(newOrderItem);
            }

            newOrder.TotalPrice = cartTotal;

            this.db.SaveChanges();

            return newOrder;
        }

        public void EmptyCart()
        {
            session.Set<List<CartItem>>(CartSessionKey, null);
        }


        public void Comment(int id, string Text)
        {
            // znajdz produkt i dodaj do karty zamowienia
            var productToAdd1 = db.Products.Where(a => a.ProductId == id).SingleOrDefault();
            Comment comment = new Comment() { CommentId=1,Description = Text, UserName = "Admin" };

            if (productToAdd1 != null)
            {
                //db.Entry(productToAdd1).Entity.Comments.Add(comment);
                productToAdd1.Comments.Add(comment);
               this.db.SaveChanges();
            }
            

            //zapisujemy ponownie stan listy w sesji
            //session.Set(CartSessionKey, productToAdd1);
        }

    }
}