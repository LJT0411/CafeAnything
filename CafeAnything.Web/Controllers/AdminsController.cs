using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
    public class AdminsController : Controller
    {
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Grab Database from repositories

        private UsersRepository AllUsers = new UsersRepository();
        private CashierRepository Table = new CashierRepository();
        private OrderCartRepository OrderCart = new OrderCartRepository();
        private CategoryRepository Category = new CategoryRepository();

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // User Methods

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            // Note : Username and Password are same
            ScryptEncoder encoder = new ScryptEncoder();

            // Find the user valid or not, and also check the role is admin or not
            var acc = AllUsers.CheckUser(user.Username, Roles.Admin);

            if (acc != null)
            {
                bool areEquals = false;
                // Compare what u typed with the hashed password which are saved inside the database
                areEquals = encoder.Compare(user.Password, acc.Password);

                // Check the password correct or not
                if (areEquals != false)
                {
                    // After Login, i created two session to store id and username
                    Session["AdminID"] = acc.ID;
                    Session["Username"] = acc.Username;
                    return RedirectToAction("Users");
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

        public ActionResult Users()
        {
            // After login , if your user list is empty, thn it will show this error msg
            if (AllUsers.GetUsers().Count() == 0)
            {
                ViewbagError("No have data");
                return View();
            }
            return View(AllUsers.GetUsers());
        }

        public ActionResult CreateNewAcc()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewAcc([Bind(Include = "ID , Username , Password , Roles")] Users user)
        {
            ScryptEncoder encoder = new ScryptEncoder();

            if (ModelState.IsValid)
            {
                try
                {
                    // hash the password
                    user.Password = encoder.Encode(user.Password);

                    CheckUserName(user.Username);

                    ViewbagSuccess("New User Created Successful.");
                    AllUsers.AddUser(user);
                    return View();
                }
                catch (Exception error)
                {
                    ViewbagError(error.Message);
                }
            }
            return View();
        }

        public ActionResult UserDetail(int? id)
        {
            // Find the user valid or not
            Users users = AllUsers.GetUser(id);
            if (users == null)
            {
                return RedirectToAction("Users");
            }
            return View(users);
        }

        public ActionResult DeleteUser(int? id)
        {
            // Find the user valid or not
            Users users = AllUsers.GetUser(id);
            if (users == null)
            {
                return RedirectToAction("Users");
            }
            // These ViewBag just for checking which user u selected to delete
            ViewBag.CheckRoles = users.Roles;

            ViewBag.CustomerRole = Roles.Customer;
            ViewBag.AdminRole = Roles.Admin;
            ViewBag.CashierRole = Roles.Cashier;
            return View(users);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {

            // If selected user is admin or cashier, it will show different msg, you can go to the view and see the if statement
            var checkAdmin = AllUsers.GetUsers().Where(c => c.ID == id && (c.Roles == Roles.Admin || c.Roles == Roles.Cashier)).SingleOrDefault();

            var checkCustomer = AllUsers.GetUsers().Where(c => c.ID == id && c.Roles == Roles.Customer).SingleOrDefault();

            if (checkAdmin != null)
            {
                AllUsers.RemoveUser(checkAdmin);
            }
            else if (checkCustomer != null)
            {
                // Find which table is reserved by this checkCustomer
                var checkTable = Table.GetTables().Where(c => c.ID == checkCustomer.ID).SingleOrDefault();

                if (checkTable != null)
                {
                    // Check which order in this table
                    var checkOrder = OrderCart.GetOrderCarts().Where(c => c.TablesID == checkTable.TablesID).ToList();

                    OrderCart.RemoveRangeOrderCart(checkOrder);
                    checkTable.TotalQuantity = 0;
                    checkTable.TotalAmount = 0;
                    checkTable.TStatus = TStatus.Empty;
                    checkTable.ID = null;
                    Table.UpdateTable(checkTable);
                    AllUsers.RemoveUser(checkCustomer);
                }
                else
                {
                    AllUsers.RemoveUser(checkCustomer);
                }
            }
            return RedirectToAction("Users");
        }

        public ActionResult EditUser(int? id)
        {
            // Find the user valid or not
            Users user = AllUsers.GetUser(id);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public ActionResult EditUser([Bind(Include = "ID , Username , Roles")] Users user)
        {
            // Get the selected user after you click the edit
            var EditUser = AllUsers.GetUsers().Where(c => c.ID == user.ID).SingleOrDefault();

            // Get other user that the username is not same as selected user name from the Users database
            var FilterUser = AllUsers.GetUsers().Where(c => c.Username != EditUser.Username).ToList();

            // Check the users name got same as what u typed got duplicate or not
            var CheckDuplicate = FilterUser.Where(c => c.Username == user.Username).SingleOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    if (CheckDuplicate != null)
                    {
                        throw new Exception("Username Duplicate");
                    }
                    EditUser.Username = user.Username;
                    EditUser.Roles = user.Roles;
                    ViewbagSuccess("Edit User Successful.");
                    AllUsers.UpdateUser(EditUser);
                    return View(EditUser);
                }
                catch (Exception lol)
                {
                    ViewbagError(lol.Message);
                }
            }
            return View(EditUser);
        }

        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult LogoutPage()
        {
            return View();
        }

        public ActionResult AjaxCreateUser(string Username, Roles Roles)
        {
            // check user after you type the username
            var CheckDuplicate = AllUsers.GetUsers().Where(c => c.Username == Username && c.Roles == Roles).SingleOrDefault();

            if (CheckDuplicate != null)
            {
                var usernameDuplicate = "Username Duplicate";
                return Json(new { Error = true, usernameDuplicate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxEditUser(int ID, string Username, Roles Roles)
        {
            // Get the selected user after you click the edit
            var EditUser = AllUsers.GetUsers().Where(c => c.ID == ID).SingleOrDefault();

            // Get other user that the username is not same as selected user name and the role from the Users database
            var FilterUser = AllUsers.GetUsers().Where(c => c.Username != EditUser.Username && c.Roles == Roles).ToList();

            // Check the users name got same as what u typed got duplicate or not
            var CheckDuplicate = FilterUser.Where(c => c.Username == Username).SingleOrDefault();

            if (CheckDuplicate != null)
            {
                var usernameDuplicate = "Username Duplicate";
                return Json(new { Error = true, usernameDuplicate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public string CheckUserName(string Username)
        {
            // Check the user database got duplicate name or not
            var checkDuplicate = AllUsers.GetUsers().Where(c => c.Username == Username).SingleOrDefault();

            if (checkDuplicate != null)
            {
                throw new Exception("User Name Duplicated");
            }
            else
            {
                return Username;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Category Methods

        public ActionResult CategoryMenu()
        {
            // Check if your category menu is empty or not
            if (Category.GetCategories().Count() == 0)
            {
                ViewbagError("No have data");
                return View(Category.GetCategories());
            }
            return View(Category.GetCategories());
        }

        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory([Bind(Include = "CategoryID,CategoryType,FoodName,Price,FoodImg")] Categories category, HttpPostedFileBase UserPhoto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set the extension only accept these 
                    var extension = new[]
                    {
                        ".jpg",".png",".jpeg",".Jpg"
                    };

                    // If you have uploaded a photo, it will go inside here
                    if (UserPhoto != null)
                    {
                        // Get the file extension
                        var ext = Path.GetExtension(UserPhoto.FileName);

                        // If the file contains which extension are set, it will go in here
                        if (extension.Contains(ext))
                        {
                            // So my database FoodImg will save the content
                            category.FoodImg = new byte[UserPhoto.ContentLength];
                            UserPhoto.InputStream.Read(category.FoodImg, 0, UserPhoto.ContentLength);
                        }
                        else
                        {
                            throw new Exception("Only accept jpg,png,jpeg. Please upload again.");
                        }

                        ViewbagSuccess("New User Created Successful.");
                        Category.AddCategory(category);
                        return View();
                    }
                    else
                    {
                        throw new Exception("Please upload your img");
                    }
                }
                catch (Exception lol)
                {
                    ViewbagError(lol.Message);
                }
            }
            return View(category);
        }

        public ActionResult CategoryDetail(int? id)
        {
            // Find the category valid or not
            Categories category = Category.GetCategory(id);
            if (category == null)
            {
                return RedirectToAction("CategoryMenu");
            }
            return View(category);
        }

        public ActionResult EditCategory(int? id)
        {
            // Find the category valid or not
            Categories category = Category.GetCategory(id);
            if (category == null)
            {
                return RedirectToAction("CategoryMenu");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory([Bind(Include = "CategoryID,CategoryType,FoodName,Price,FoodImg")] Categories category, HttpPostedFileBase UserPhoto)
        {
            if (ModelState.IsValid)
            {
                // Get the category that u selected
                var EditCategory = Category.GetCategories().Where(c => c.CategoryID == category.CategoryID).SingleOrDefault();

                try
                {
                    var extension = new[]
                        {
                             ".jpg",".png",".jpeg",".Jpg"
                        };

                    if (UserPhoto == null)
                    {
                        category.FoodImg = EditCategory.FoodImg;
                    }
                    else
                    {
                        var ext = Path.GetExtension(UserPhoto.FileName);

                        if (extension.Contains(ext))
                        {
                            category.FoodImg = new byte[UserPhoto.ContentLength];
                            UserPhoto.InputStream.Read(category.FoodImg, 0, UserPhoto.ContentLength);
                        }
                        else
                        {
                            ViewbagError("Only accept jpg,png,jpeg. Please upload again.");
                            return View(category);
                        }
                    }
                    EditCategory.CategoryType = category.CategoryType;
                    EditCategory.FoodName = category.FoodName;
                    EditCategory.Price = category.Price;
                    EditCategory.FoodImg = category.FoodImg;
                    ViewbagSuccess("Edit Category Successful.");
                    Category.Save();
                    return View(EditCategory);
                }
                catch (Exception lol)
                {
                    ViewbagError(lol.Message);
                }
            }
            return View(category);
        }

        public ActionResult DeleteCategory(int? id)
        {
            // Find the category valid or not
            Categories category = Category.GetCategory(id);
            if (category == null)
            {
                return RedirectToAction("CategoryMenu");
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            // Find the selected category from the database
            var FindCategory = Category.GetCategories().Where(c => c.CategoryID == id).SingleOrDefault();

            // Find which order is related to the selected category
            var checkOrderCart = OrderCart.GetOrderCarts().Where(c => c.CategoryID == FindCategory.CategoryID).ToList();


            if (checkOrderCart != null)
            {
                foreach (var item in checkOrderCart)
                {
                    // Find which table is ordered this selected category
                    var checkTable = Table.GetTables().Where(c => c.TablesID == item.TablesID).SingleOrDefault();
                    checkTable.TotalQuantity -= item.Quantity;
                    checkTable.TotalAmount -= item.TotalAmount;
                    item.CategoryID = FindCategory.CategoryID;

                    // After minus the quantity and amount if the quantity become 0 thn will go in this if statement
                    if (checkTable.TotalQuantity == 0)
                    {
                        checkTable.TStatus = TStatus.Empty;
                        checkTable.TotalQuantity = 0;
                        checkTable.TotalAmount = 0;
                        checkTable.ID = null;
                    }
                    // Update the table data first
                    Table.UpdateTable(checkTable);
                }
                // Then remove the orders
                OrderCart.RemoveRangeOrderCart(checkOrderCart);
            }
            // Then remove Category
            Category.RemoveCategory(FindCategory);

            return RedirectToAction("CategoryMenu");
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
