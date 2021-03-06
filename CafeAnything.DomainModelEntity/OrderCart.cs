﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CafeAnything.DomainModelEntity
{
    public class OrderCart
    {
        [Key]
        public int OrderID { get; set; }

        public int Quantity { get; set; }

        public double TotalAmount { get; set; }

        public int? TablesID { get; set; }

        public int? CategoryID { get; set; }

        public virtual Categories Categories { get; set; }

        public virtual Tables Tables { get; set; }
    }
}
