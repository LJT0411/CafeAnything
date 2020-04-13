using CafeAnything.DomainModel;
using CafeAnything.DomainModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.Persistence.Repositories
{
    public class OrderCartRepository : IOrderCart
    {
        private AppDbContext db = new AppDbContext();

        /////////////////////////////////////////////////////////////////////////////////////////////////
        // CRUD 

        public void AddOrderCart(OrderCart ordercart)
        {
            db.OrderCarts.Add(ordercart);
            Save();
        }

        public OrderCart GetOrderCart(int? id)
        {
            OrderCart ordercart = db.OrderCarts.Find(id);
            return ordercart;
        }

        public IEnumerable<OrderCart> GetOrderCarts()
        {
            return db.OrderCarts.ToList();
        }

        public void UpdateOrderCart(OrderCart ordercart)
        {
            db.Entry(ordercart).State = EntityState.Modified;
            Save();
        }

        public void RemoveOrderCart(OrderCart odercart)
        {
            db.OrderCarts.Remove(odercart);
            Save();
        }

        public void RemoveRangeOrderCart(IEnumerable<OrderCart> ordercart)
        {
            db.OrderCarts.RemoveRange(ordercart);
            Save();
        }

        public void Save()
        {
            db.SaveChanges();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
