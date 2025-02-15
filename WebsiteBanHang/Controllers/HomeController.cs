using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WebsiteBanHang.Connect;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        public ActionResult Index()
        {
            var lstCategory = objWebsiteBanHangEntities.Categories.ToList();
            var lstProduct = objWebsiteBanHangEntities.Products.ToList();
            var lstBrand = objWebsiteBanHangEntities.Brands.ToList();

            var model = new HomeModel
            {
                categories = lstCategory,
                products = lstProduct,
                brands = lstBrand
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Connect.User _user)
        {
            //Kiem tra va luu vao db
            if (ModelState.IsValid)
            {
                var check = objWebsiteBanHangEntities.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    _user.Password = GetMD5(_user.Password);
                    objWebsiteBanHangEntities.Configuration.ValidateOnSaveEnabled = false;
                    objWebsiteBanHangEntities.Users.Add(_user);
                    objWebsiteBanHangEntities.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Email đã tồn tại.";
                    return View();
                }
            }
            return View();
        }
        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {


                var f_password = GetMD5(model.Password);
                var data = objWebsiteBanHangEntities.Users.Where(s => (s.UserName.Equals(model.EmailOrUsername) || s.Email.Equals(model.EmailOrUsername)) && s.Password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["FullName"] = data.FirstOrDefault().LastName + " " + data.FirstOrDefault().FirstName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["idUser"] = data.FirstOrDefault().Id;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }

        public ActionResult Blank_Starter()
        {
            return View();
        }
        public ActionResult Content()
        {
            return View();
        }
        public ActionResult Listing_Gird(string SearchString, string currentFilter, int? page)
        {
            if (SearchString == null)
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;

            var products = objWebsiteBanHangEntities.Products.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(n => n.Name.Contains(SearchString));
            }

            products = products.OrderByDescending(n => n.Id);

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Listing_Large(string SearchString, string currentFilter, int? page)
        {
            if (SearchString == null)
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;

            var products = objWebsiteBanHangEntities.Products.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(n => n.Name.Contains(SearchString));
            }

            products = products.OrderByDescending(n => n.Id);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Shopping_Cart()
        {
            return View();
        }
    }
}