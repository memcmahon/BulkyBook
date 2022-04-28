using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Facades
{
    public class OrderFacade
    {
        OrderHeader OrderHeader { get; set; }
        IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
