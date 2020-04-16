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
    public class CashiersController : Controller
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Grab Database from repositories

        private UsersRepository AllUsers = new UsersRepository();
        private CashierRepository Table = new CashierRepository();
        private OrderCartRepository OrderCart = new OrderCartRepository();

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Cashier Methods

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            // Note : Username and Password are same
            ScryptEncoder encoder = new ScryptEncoder();

            // Find the user valid or not, and also check the role is cashier or not
            var acc = AllUsers.CheckUser(user.Username, Roles.Cashier);

            if (acc != null)
            {
                bool areEquals = false;
                // Compare what u typed with the hashed password which are saved inside the database
                areEquals = encoder.Compare(user.Password, acc.Password);

                // Check the password correct or not
                if (areEquals != false)
                {
                    // After Login, i created two session to store id and username
                    Session["CashierID"] = acc.ID;
                    Session["Username"] = acc.Username;
                    return RedirectToAction("Tables");
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

        public ActionResult Tables()
        {
            // When you login as cashier, if thr doesn't have table, thn it will go inside this if statement
            if (Table.GetTables().Count() == 0)
            {
                ViewbagError("No have tables");
                return View();
            }
            return View(Table.GetTables());
        }

        public ActionResult AddTable()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTable([Bind(Include = "TablesID,TableNo,TStatus,TotalAmount,TotalQuantity,ID")] Tables table)
        {
            // To check if there got duplicate Table No
            var checkTable = Table.GetTables().Where(c => c.TableNo == table.TableNo).SingleOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    if (checkTable != null)
                    {
                        throw new Exception("Table No Duplicated");
                    }
                    table.TotalAmount = 0;
                    table.TotalQuantity = 0;
                    table.ID = null;

                    ViewbagSuccess("New Table Created Successful.");
                    Table.AddTable(table);
                    return View();
                }
                catch (Exception error)
                {
                    ViewbagError(error.Message);
                }
            }
            return View();
        }

        public ActionResult EditTable(int? id)
        {
            // To find which table you are going to edit
            Tables table = Table.GetTable(id);
            if (table != null)
            {
                return View(table);
            }
            return RedirectToAction("Tables");
        }

        [HttpPost]
        public ActionResult EditTable([Bind(Include = "TablesID,TableNo,TStatus,TotalAmount,TotalQuantity,ID")] Tables table)
        {

            // Get the selected table after you click the edit
            var EditTable = Table.GetTables().Where(c => c.TablesID == table.TablesID).SingleOrDefault();

            // Get other table that the Table No is not same as selected Table No from the Tables database
            var FilterTable = Table.GetTables().Where(c => c.TableNo != EditTable.TableNo).ToList();

            // Check the Table No got same as what u typed got duplicate or not
            var CheckDuplicate = FilterTable.Where(c => c.TableNo == table.TableNo).SingleOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckDuplicate != null)
                    {
                        throw new Exception("Table No Duplicate");
                    }

                    EditTable.TableNo = table.TableNo;
                    EditTable.TStatus = table.TStatus;
                    if (EditTable.TStatus == TStatus.Empty)
                    {
                        EditTable.TotalQuantity = 0;
                        EditTable.TotalAmount = 0;
                        EditTable.ID = null;
                    }
                    ViewbagSuccess("Edit Table Successful.");
                    Table.UpdateTable(EditTable);
                    return View(EditTable);
                }
                catch (Exception lol)
                {
                    ViewbagError(lol.Message);
                }
            }
            return View(table);
        }

        public ActionResult TableDetail(int? id)
        {
            // To find which table you are going to see
            Tables table = Table.GetTable(id);
            if (table == null)
            {
                return RedirectToAction("Tables");
            }
            return View(table);
        }

        public ActionResult DeleteTable(int? id)
        {
            // To find which table you are going to delete
            Tables table = Table.GetTable(id);
            if (table == null)
            {
                return RedirectToAction("Tables");
            }
            ViewBag.DeleteWarning = "Note : Selected Table which Orders will be deleted.";
            return View(table);
        }

        [HttpPost, ActionName("DeleteTable")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTableConfirmed(int id)
        {
            // To find which table you are going to delete
            var checkTable = Table.GetTables().Where(c => c.TablesID == id).SingleOrDefault();

            if (checkTable != null)
            {
                // To find which order is related to the selected table
                var checkOrder = OrderCart.GetOrderCarts().Where(c => c.TablesID == id).ToList();
                if (checkOrder != null)
                {
                    // If selected table got orders, thn it will remove all orders
                    OrderCart.RemoveRangeOrderCart(checkOrder);
                }
                Table.RemoveTable(checkTable);
            }
            return RedirectToAction("Tables");
        }

        public ActionResult AjaxCreateTable(string TableNo)
        {
            // To check if there got duplicate Table No
            var CheckDuplicate = Table.GetTables().Where(c => c.TableNo == TableNo).SingleOrDefault();

            if (CheckDuplicate != null)
            {
                var TableNoDuplicate = "Table No Duplicate";
                return Json(new { Error = true, TableNoDuplicate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxEditTable(int TablesID, string TableNo)
        {
            // Get the selected table after you click the edit
            var EditTable = Table.GetTables().Where(c => c.TablesID == TablesID).SingleOrDefault();

            // Get other table that the Table No is not same as selected Table No from the Tables database
            var FilterTable = Table.GetTables().Where(c => c.TableNo != EditTable.TableNo).ToList();

            // Check the Table No got same as what u typed got duplicate or not
            var CheckDuplicate = FilterTable.Where(c => c.TableNo == TableNo).SingleOrDefault();

            if (CheckDuplicate != null)
            {
                var TableNoDuplicate = "Table No Duplicate";
                return Json(new { Error = true, TableNoDuplicate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
            }
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
