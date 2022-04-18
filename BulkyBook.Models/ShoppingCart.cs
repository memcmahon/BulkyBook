using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        [Range(1, 1000)]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public double cartPrice()
        {
            double cartPrice;

            switch (Count)
            {
                case > 99:
                    cartPrice = Product.Price100;
                    break;
                case > 49:
                    cartPrice = Product.Price50;
                    break;
                default:
                    cartPrice = Product.Price;
                    break;
            }

            return cartPrice;
        }

        public void IncreaseQuantity()
        {
            Count++;
        }

        public void DecreaseQuantity()
        {
            Count--;
        }
    }
}
