using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CafeAnything.DomainModelEntity
{
    public class Tables
    {
        [Key]
        public int TablesID { get; set; }

        public string TableNo { get; set; }

        public TStatus TStatus { get; set; }

        public int TotalQuantity { get; set; }

        public double TotalAmount { get; set; }

        public int? ID { get; set; }

        public virtual Users Users { get; set; }

        public ICollection<OrderCart> OrderCarts { get; set; }
    }

    public enum TStatus
    {
        Empty, Occupied
    }
}
