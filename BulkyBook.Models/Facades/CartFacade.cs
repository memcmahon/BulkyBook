using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Facades
{
    public class CartFacade
    {
        public IEnumerable<ShoppingCart> CartItems { get; set; }
        public OrderHeader OrderHeader { get; set; }

        public double Total()
        {
            double total = 0;

            foreach (var item in CartItems)
            {
                total += item.cartPrice() * item.Count;
            }

            return total;
        }
    }

}
