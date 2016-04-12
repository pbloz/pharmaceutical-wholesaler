using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Hurtownia.DAL;
using Hurtownia.Models;

namespace Hurtownia.Infrastructure
{
    public class ProductDetailsDynamicNodeProvider : DynamicNodeProviderBase
    {
        private StoreContext _db = new StoreContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();

            foreach (Product a in _db.Products)
            {
                DynamicNode n = new DynamicNode();
                n.Title = a.ProductName;
                n.Key = "Product_" + a.ProductId;
                n.ParentKey = "Category_" + a.CategoryId;
                n.RouteValues.Add("id", a.ProductId);
                returnValue.Add(n);
            }

            // Return 
            return returnValue;
        }
    }
}