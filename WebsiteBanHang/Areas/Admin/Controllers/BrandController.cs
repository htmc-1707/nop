using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanHang.Connect;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        // GET: Admin/Brand
        public ActionResult Index(string SearchString, string currentFilter, int? page)
        {
            if (SearchString == null)
            {
                SearchString = currentFilter;
            }
            
            ViewBag.CurrentFilter = SearchString;

            var brands = objWebsiteBanHangEntities.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                brands = brands.Where(n => n.Name.Contains(SearchString));
            }

            brands = brands.OrderByDescending(n => n.Id);

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(brands.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Brand/Details
        public ActionResult Details(int id)
        {
            var objBrand = objWebsiteBanHangEntities.Brands.Where(n => n.Id == id).FirstOrDefault();
            return View(objBrand);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand, HttpPostedFileBase Image)
        {
            if (Image == null)
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
                        if (Image != null)
                        {
                            // Lấy tên file gốc
                            string fileName = Path.GetFileName(Image.FileName);

                            // Lưu file vào thư mục trên server (không lưu đường dẫn)
                            string savePath = Path.Combine(Server.MapPath("~/Content/images/brand/"), fileName);
                            Image.SaveAs(savePath);

                            // Chỉ lưu tên file vào objProduct
                            brand.Image = fileName;
                        }

                        // Thêm sản phẩm vào cơ sở dữ liệu
                        objWebsiteBanHangEntities.Brands.Add(brand);
                        objWebsiteBanHangEntities.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    }
                }
                return View(brand);
            }
        }

        // GET: Admin/Brand/Edit
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Brand brand = objWebsiteBanHangEntities.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }

            return View(brand);
        }

        // POST: Admin/Brand/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Brand brand, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Đường dẫn tới thư mục chứa ảnh
                    string imagePath = Server.MapPath("~/Content/images/brand/");

                    // Nếu người dùng upload ảnh mới
                    if (Image != null)
                    {
                        // Lấy tên file gốc của ảnh mới
                        string newFileName = Path.GetFileName(Image.FileName);

                        // Đường dẫn lưu ảnh mới
                        string newFilePath = Path.Combine(imagePath, newFileName);

                        // Lưu ảnh mới vào thư mục trên máy chủ
                        Image.SaveAs(newFilePath);

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
                        brand.Image = newFileName;
                    }
                    else
                    {
                        brand.Image = Request.Form["oldimage"];
                    }
                    // Cập nhật thông tin danh mục
                    objWebsiteBanHangEntities.Entry(brand).State = EntityState.Modified;
                    objWebsiteBanHangEntities.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cập nhật không thành công: " + ex.Message);
                }
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        // GET: Admin/Brand/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = objWebsiteBanHangEntities.Brands.Find(id);
            if (brand == null)
            {
                return HttpNotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brand/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Tìm danh mục cần xóa
                Brand brand = objWebsiteBanHangEntities.Brands.Find(id);
                if (brand == null)
                {
                    return HttpNotFound();
                }

                // Lấy đường dẫn ảnh của danh mục
                string imagePath = Server.MapPath("~/Content/images/brand/");
                string imageFile = brand.Image;

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
                objWebsiteBanHangEntities.Brands.Remove(brand);
                objWebsiteBanHangEntities.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể xóa thương hiệu: " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}