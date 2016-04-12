using Hurtownia.DAL;
using Hurtownia.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hurtownia.Infrastructure
{

    public class ProductListDynamicNodeProvider : DynamicNodeProviderBase
    {
        private StoreContext _db = new StoreContext();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            // Build value 
            var returnValue = new List<DynamicNode>();

            foreach (Category g in _db.Categories)
            {
                DynamicNode n = new DynamicNode();
                n.Title = g.NameCategory;
                n.Key = "Category_" + g.CategoryId;
                n.RouteValues.Add("categoryname", g.NameCategory);
                returnValue.Add(n);
            }

            // Return 
            return returnValue;
        }
    }
}