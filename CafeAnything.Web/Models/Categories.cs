using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CafeAnything.Web.Models
{
    public class Categories
    {
        [Key]
        public int CategoryID { get; set; }

        public CategoryType CategoryType { get; set; }

        public string FoodName { get; set; }

        public double Price { get; set; }

        public byte[] FoodImg { get; set; }

        public virtual ICollection<OrderCart> OrderCarts { get; set; }

    }

    public enum CategoryType
    {
        Food, Drink , Dessert
    }
}