﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CafeAnything.Web.Models
{
    public class Users
    {
        [Key]
        public int ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Roles Roles { get; set; }

        public virtual ICollection<Tables> Tables { get; set; }
    }

    public enum Roles
    {
        Admin,Customer,Cashier
    }
}