using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CafeAnything.DomainModel;
using CafeAnything.Persistence;
using CafeAnything.Persistence.Repositories;
//using CafeAnything.Web.Models;
using Scrypt;

namespace CafeAnything.Web.Controllers
{
    public class CustomersController : Controller
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Grab Database from repositories

        private UsersRepository AllUsers = new UsersRepository();
        private CashierRepository Table = new CashierRepository();
        private OrderCartRepository OrderCart = new OrderCartRepository();
        private CategoryRepository Category = new CategoryRepository();

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Customer Methods

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            // Note : Username and Password are same
            ScryptEncoder encoder = new ScryptEncoder();

            // Find the user valid or not, and also check the role is customer or not
            var acc = AllUsers.CheckUser(user.Username, Roles.Customer);

            if (acc != null)
            {
                bool areEquals = false;
                // Compare what u typed with the hashed password which are saved inside the database
                areEquals = encoder.Compare(user.Password, acc.Password);

                // Check the password correct or not
                if (areEquals != false)
                {
                    // After Login, i created two session to store id and username
                    Session["CustomerID"] = acc.ID;
                    Session["Username"] = acc.Username;

                    return RedirectToAction("Menu");
                }
                else
                {
                    ViewbagError("Invalid Username or Password");
                }
                return View();
            }
            else
            {
                ViewbagError("Invalid Username or Password");
            }
            return View();
        }

        public ActionResult Menu()
        {
            // Get the login user id
            var GetUser = Convert.ToInt32(Session["CustomerID"]);

            // After login , if category list is empty, thn it will show this error msg
            if (Category.GetCategories().Count() == 0)
            {
                ViewbagError("No have data.");
                return View();
            }

            // Find which table foreign key of ID is related to GetUser
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();

            if (checkTable != null)
            {
                // Find which order is related to the checkTable
                var GetOrderCount = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();
                var TotalOrder = 0;
                double TotalAmount = 0;

                // Loop over the orders to get the total of order and amount
                foreach (var item in GetOrderCount)
                {
                    TotalOrder += item.Quantity;
                    TotalAmount += item.TotalAmount;
                }
                ViewBag.MyOrder = TotalOrder;
                ViewBag.TotalAmount = TotalAmount;
            }
            else
            {
                ViewBag.MyOrder = 0;
            }
            return View(Category.GetCategories());
        }

        public ActionResult ClickOrder(int? id)
        {
            // Get the login user id
            var GetUser = Convert.ToInt32(Session["CustomerID"]);

            // Find which category u clicked
            Categories category = Category.GetCategory(id);

            if (category == null)
            {
                return RedirectToAction("Menu");
            }
            // Grab the ordercart properties
            OrderCart AddOrder = new OrderCart();

            // Find which table foreign key of ID is related to GetUser
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();

            var TotalOrder = 0;
            double TotalAmount = 0;

            if (checkTable != null)
            {
                // Find which orders are related to the category and the table
                OrderCart ordercart = OrderCart.GetOrderCarts().Where(c => c.CategoryID == id && c.TablesID == checkTable.TablesID).SingleOrDefault();

                if (ordercart == null)
                {
                    AddOrder.Quantity = 1;
                    AddOrder.TotalAmount += category.Price;
                    AddOrder.CategoryID = category.CategoryID;
                    AddOrder.TablesID = checkTable.TablesID;
                    OrderCart.AddOrderCart(AddOrder);
                }
                else
                {
                    ordercart.Quantity += 1;
                    ordercart.TotalAmount += category.Price;
                    OrderCart.UpdateOrderCart(ordercart);
                }
                checkTable.TotalQuantity += 1;
                checkTable.TotalAmount += category.Price;
                Table.UpdateTable(checkTable);
            }
            else
            {
                // Check the enough table or not
                var checkTables = Table.GetTables().Where(c => c.TStatus == TStatus.Occupied).ToList();
                
                if (Table.GetTables().Count() == checkTables.Count())
                {
                    Session["NoTable"] = "Sorry, currently is full. Please wait for a few mins.";
                    return RedirectToAction("Menu");
                }

                AddOrder.Quantity = 1;
                AddOrder.TotalAmount += category.Price;
                AddOrder.CategoryID = category.CategoryID;

                TotalOrder++;
                TotalAmount += category.Price;
                
                // Loop over the tables and check which table status is empty
                foreach (var item in Table.GetTables())
                {
                    if (item.TStatus == TStatus.Empty)
                    {
                        AddOrder.TablesID = item.TablesID;
                        item.ID = GetUser;
                        item.TotalQuantity = 1;
                        item.TotalAmount += category.Price;
                        item.TStatus = TStatus.Occupied;
                        Table.UpdateTable(item);
                        break;
                    }

                    
                }
                OrderCart.AddOrderCart(AddOrder);
            }

            var checkTableAfterAdd = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();

            // Find which order is related to the checkTable 
            var GetOrderCount = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTableAfterAdd.TablesID).ToList();
            // Loop over the orders, and get the total of order and amount
            foreach (var item in GetOrderCount)
            {
                TotalOrder += item.Quantity;
            }

            Session["NewOrder"] = category.FoodName + " added into Order";
            ViewBag.MyOrder = TotalOrder;

            return RedirectToAction("Menu");
        }

        public ActionResult MyOrder()
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            int TotalOrder = 0;
            double TotalAmount = 0;

            if (checkTable != null)
            {
                var OrderList = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();

                foreach (var item in OrderList)
                {
                    TotalOrder += item.Quantity;
                    TotalAmount += item.TotalAmount;
                }
                ViewBag.MyOrder = TotalOrder;
                ViewBag.TotalAmount = TotalAmount;
                return View(OrderList);

            }
            ViewBag.Error = "No have order. Go menu order some thing.";
            ViewBag.MyOrder = 0;

            return View();
        }

        public ActionResult PlusOrder(int? id)
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get category
            Categories category = Category.GetCategory(id);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            if (checkTable != null)
            {
                // Find which order is related to the category id and tables id
                OrderCart ordercart = OrderCart.GetOrderCarts().Where(c => c.CategoryID == id && c.TablesID == checkTable.TablesID).SingleOrDefault();

                ordercart.Quantity++;
                ordercart.TotalAmount += category.Price;
                checkTable.TotalQuantity++;
                checkTable.TotalAmount += category.Price;
                OrderCart.UpdateOrderCart(ordercart);
                Table.UpdateTable(checkTable);
            }
            return RedirectToAction("MyOrder");
        }

        public ActionResult MinusOrder(int? id)
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get category
            Categories category = Category.GetCategory(id);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            if (checkTable != null)
            {
                // Find which order is related to the category id and tables id
                OrderCart ordercart = OrderCart.GetOrderCarts().Where(c => c.CategoryID == id && c.TablesID == checkTable.TablesID).SingleOrDefault();
                
                // After minus the quantity and amount if the quantity become 0 thn will go in this if statement
                if (ordercart.Quantity != 1)
                {
                    ordercart.Quantity--;
                    ordercart.TotalAmount -= category.Price;
                    checkTable.TotalQuantity--;
                    checkTable.TotalAmount -= category.Price;
                }
                OrderCart.UpdateOrderCart(ordercart);
                Table.UpdateTable(checkTable);
            }
            return RedirectToAction("MyOrder");
        }

        public ActionResult DeleteOrder(int? id)
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get category
            Categories category = Category.GetCategory(id);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            if (checkTable != null)
            {
                // Find which order is related to the category id and tables id
                OrderCart ordercart = OrderCart.GetOrderCarts().Where(c => c.CategoryID == id && c.TablesID == checkTable.TablesID).SingleOrDefault();

                OrderCart.RemoveOrderCart(ordercart);
                checkTable.TotalQuantity -= ordercart.Quantity;
                checkTable.TotalAmount -= ordercart.TotalAmount;
                // After minus the quantity and amount if the quantity become 0 thn will go in this if statement
                if (checkTable.TotalQuantity == 0)
                {
                    checkTable.TStatus = TStatus.Empty;
                    checkTable.TotalQuantity = 0;
                    checkTable.TotalAmount = 0;
                    checkTable.ID = null;
                }
                Table.UpdateTable(checkTable);
            }
            return RedirectToAction("MyOrder");
        }

        public ActionResult CancelOrder()
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            if (checkTable != null)
            {
                // Find which orders is related to the category id and tables id
                var ordercart = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();

                OrderCart.RemoveRangeOrderCart(ordercart);
                checkTable.TotalQuantity = 0;
                checkTable.TotalAmount = 0;
                checkTable.ID = null;
                checkTable.TStatus = TStatus.Empty;
                Table.UpdateTable(checkTable);
            }
            return RedirectToAction("MyOrder");
        }

        public ActionResult ConfirmOrder()
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            int TotalOrder = 0;
            double TotalAmount = 0;

            if (checkTable != null)
            {
                var OrderList = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();

                foreach (var item in OrderList)
                {
                    TotalOrder += item.Quantity;
                    TotalAmount += item.TotalAmount;
                }
                ViewBag.MyOrder = TotalOrder;
                ViewBag.TotalAmount = TotalAmount;
                return View(OrderList);
            }
            ViewBag.MyOrder = 0;
            return View();
        }

        public ActionResult ConfirmPayment()
        {
            var GetUser = Convert.ToInt32(Session["CustomerID"]);
            // Get Table which foreign key id is GetUser id
            var checkTable = Table.GetTables().Where(c => c.ID == GetUser).SingleOrDefault();
            if (checkTable != null)
            {
                // Find which orders is related to the category id and tables id
                var ordercart = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();

                OrderCart.RemoveRangeOrderCart(ordercart);
                checkTable.TotalQuantity = 0;
                checkTable.TotalAmount = 0;
                checkTable.ID = null;
                checkTable.TStatus = TStatus.Empty;
                Table.UpdateTable(checkTable);
            }
            ViewBag.Success = "Successful Transaction. Thanks you and come visit again!";
            ViewBag.MyOrder = 0;
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult LogoutPage()
        {
            return View();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Non Actions

        [NonAction]
        public void ViewbagError(string Msg)
        {
            ViewBag.Error = Msg;
        }

        [NonAction]
        public void ViewbagSuccess(string Msg)
        {
            ViewBag.Success = Msg;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
