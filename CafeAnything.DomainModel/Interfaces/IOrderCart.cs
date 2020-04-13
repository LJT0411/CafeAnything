using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeAnything.DomainModel.Interfaces
{
    public interface IOrderCart
    {
        IEnumerable<OrderCart> GetOrderCarts();

        OrderCart GetOrderCart(int? id);

        void AddOrderCart(OrderCart ordercart);

        void UpdateOrderCart(OrderCart ordercart);

        void RemoveOrderCart(OrderCart odercart);

        void RemoveRangeOrderCart(IEnumerable<OrderCart> ordercart);

        void Save();

    }
}
