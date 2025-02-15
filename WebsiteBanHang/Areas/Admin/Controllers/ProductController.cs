using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebsiteBanHang.Connect;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        // GET: Admin/Product
        public ActionResult Index(string SearchString, string currentFilter, int? page)
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

        //GET: Admin/Product/Details
        public ActionResult Details(int id)
        {
            var objProduct = objWebsiteBanHangEntities.Products.Where(n => n.Id == id).FirstOrDefault();
            return View(objProduct);
        }

        //GET: Admin/Product/Create
        [HttpGet]
        public ActionResult Create()
        {
            //lấy dữ liệu danh mục
            ViewBag.CategoryList = new SelectList(objWebsiteBanHangEntities.Categories.ToList().OrderBy(n=>n.Name),"Id", "Name");
            //lấy dữ liệu thương hiệu
            ViewBag.BrandList = new SelectList(objWebsiteBanHangEntities.Brands.ToList().OrderBy(n => n.Name), "Id", "Name");
            return View();
        }

        //POST: Admin/Product/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product objProduct, HttpPostedFileBase ImageUpload, ProductViewModel model)
        {
            ViewBag.CategoryList = new SelectList(objWebsiteBanHangEntities.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");
            ViewBag.BrandList = new SelectList(objWebsiteBanHangEntities.Brands.ToList().OrderBy(n => n.Name), "Id", "Name");
            if (ImageUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh.";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Xử lý file upload (nếu có)
                        if (ImageUpload != null)
                        {
                            // Lấy tên file gốc
                            string fileName = Path.GetFileName(ImageUpload.FileName);

                            // Lưu file vào thư mục trên server (không lưu đường dẫn)
                            string savePath = Path.Combine(Server.MapPath("~/Content/images/product/"), fileName);
                            ImageUpload.SaveAs(savePath);

                            // Chỉ lưu tên file vào objProduct
                            objProduct.Images = fileName;
                        }

                        // Thêm sản phẩm vào cơ sở dữ liệu
                        objWebsiteBanHangEntities.Products.Add(objProduct);
                        objWebsiteBanHangEntities.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    }
                }

                return View(model);
            }
        }

        // GET: Admin/Product/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = objWebsiteBanHangEntities.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Đặt Description về trống khi vào trang Edit
            product.Description = "";

            // Đặt thêm phần chi tiết về trống khi vào trang Edit
            product.Details = "";

            // Lấy dữ liệu danh mục và thương hiệu
            ViewBag.CategoryList = new SelectList(objWebsiteBanHangEntities.Categories.ToList().OrderBy(n => n.Name), "Id", "Name");
            ViewBag.BrandList = new SelectList(objWebsiteBanHangEntities.Brands.ToList().OrderBy(n => n.Name), "Id", "Name");

            return View(product);
        }


        // POST: Admin/Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Đường dẫn tới thư mục chứa ảnh
                    string imagePath = Server.MapPath("~/Content/images/product/");

                    // Nếu người dùng upload ảnh mới
                    if (ImageUpload != null)
                    {
                        // Lấy tên file gốc của ảnh mới
                        string newFileName = Path.GetFileName(ImageUpload.FileName);

                        // Đường dẫn lưu ảnh mới
                        string newFilePath = Path.Combine(imagePath, newFileName);

                        // Lưu ảnh mới vào thư mục trên máy chủ
                        ImageUpload.SaveAs(newFilePath);

                        // Xóa ảnh cũ nếu tồn tại
                        string oldFileName = Request.Form["oldimage"];
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            string oldFilePath = Path.Combine(imagePath, oldFileName);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath); // Xóa ảnh cũ
                            }
                        }

                        // Cập nhật thông tin file ảnh mới vào đối tượng
                        product.Images = newFileName;
                    }
                    else
                    {
                        product.Images = Request.Form["oldimage"];
                    }
                    // Cập nhật thông tin danh mục
                    objWebsiteBanHangEntities.Entry(product).State = EntityState.Modified;
                    objWebsiteBanHangEntities.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cập nhật không thành công: " + ex.Message);
                }
                return RedirectToAction("Index");
            }
            // Nếu không thành công, hiển thị lại view với dữ liệu cần thiết
            ViewBag.CategoryList = new SelectList(objWebsiteBanHangEntities.Categories.ToList().OrderBy(n => n.Name), "Id", "Name", product.CategoryId);
            ViewBag.BrandList = new SelectList(objWebsiteBanHangEntities.Brands.ToList().OrderBy(n => n.Name), "Id", "Name", product.BrandId);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = objWebsiteBanHangEntities.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Tìm danh mục cần xóa
                Product product = objWebsiteBanHangEntities.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }

                // Lấy đường dẫn ảnh của danh mục
                string imagePath = Server.MapPath("~/Content/images/product/");
                string imageFile = product.Images;

                // Kiểm tra và xóa file ảnh nếu tồn tại
                if (!string.IsNullOrEmpty(imageFile))
                {
                    string fullPath = Path.Combine(imagePath, imageFile);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath); // Xóa ảnh
                    }
                }

                // Xóa danh mục khỏi cơ sở dữ liệu
                objWebsiteBanHangEntities.Products.Remove(product);
                objWebsiteBanHangEntities.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể xóa sản phẩm: " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}