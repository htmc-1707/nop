using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Connect;

namespace WebsiteBanHang.Controllers
{
    public class ProductController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        // GET: Product
        public ActionResult Detail(int id)
        {
            try
            {
                var product = objWebsiteBanHangEntities.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy sản phẩm với ID: {id}";
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
                return View("Error");
            }
        }
    }
}