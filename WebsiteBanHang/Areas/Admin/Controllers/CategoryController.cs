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
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        WebsiteBanHangEntities objWebsiteBanHangEntities = new WebsiteBanHangEntities();
        // GET: Admin/Category
        public ActionResult Index(string SearchString, string currentFilter, int? page)
        {
            if (SearchString == null)
            {
                SearchString = currentFilter;
            }

            ViewBag.CurrentFilter = SearchString;

            var categories = objWebsiteBanHangEntities.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(SearchString))
            {
                categories = categories.Where(n => n.Name.Contains(SearchString));
            }

            categories = categories.OrderByDescending(n => n.Id);

            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(categories.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(int id)
        {
            var objCategory = objWebsiteBanHangEntities.Categories.Where(n => n.Id == id).FirstOrDefault();
            return View(objCategory);
        }

        [HttpGet]
        public ActionResult Create()
        {
            // Tạo SelectList cho ParentId (chỉ lấy các danh mục không phải là con của chính nó)
            var categories = objWebsiteBanHangEntities.Categories.Where(c => c.ParentId == null).OrderBy(n => n.Name).ToList();
            ViewBag.ParentId = new SelectList(categories, "Id", "Name");
                
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category, HttpPostedFileBase Image)
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
                            string savePath = Path.Combine(Server.MapPath("~/Content/images/categories/"), fileName);
                            Image.SaveAs(savePath);

                            // Chỉ lưu tên file vào objProduct
                            category.Image = fileName;
                        }

                        // Thêm sản phẩm vào cơ sở dữ liệu
                        objWebsiteBanHangEntities.Categories.Add(category);
                        objWebsiteBanHangEntities.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    }
                }
                return View(category);
            }
        }
        // GET: Category/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = objWebsiteBanHangEntities.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            // Tạo SelectList cho ParentId (chỉ lấy các danh mục không phải là con của chính nó)
            var categories = objWebsiteBanHangEntities.Categories.Where(c => c.Id != id && c.ParentId == null).OrderBy(n => n.Name).ToList();
            // Thêm mục tùy chọn trống vào danh sách
            categories.Insert(0, new Category { Id = 0, Name = "--- Chọn danh mục cha ---" });
            ViewBag.ParentId = new SelectList(categories, "Id", "Name", category.ParentId);

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Đường dẫn tới thư mục chứa ảnh
                    string imagePath = Server.MapPath("~/Content/images/categories/");

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
                        category.Image = newFileName;
                    }
                    else
                    {
                        category.Image = Request.Form["oldimage"];
                    }
                    // Cập nhật thông tin danh mục
                    objWebsiteBanHangEntities.Entry(category).State = EntityState.Modified;
                    objWebsiteBanHangEntities.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cập nhật không thành công: " + ex.Message);
                }
                return RedirectToAction("Index");
            }
            var categories = objWebsiteBanHangEntities.Categories.Where(c => c.Id != category.Id && c.ParentId == null).OrderBy(n => n.Name).ToList();
            ViewBag.ParentId = new SelectList(categories, "Id", "Name", category.ParentId);
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = objWebsiteBanHangEntities.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Tìm danh mục cần xóa
                Category category = objWebsiteBanHangEntities.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                // Lấy đường dẫn ảnh của danh mục
                string imagePath = Server.MapPath("~/Content/images/categories/");
                string imageFile = category.Image;

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
                objWebsiteBanHangEntities.Categories.Remove(category);
                objWebsiteBanHangEntities.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể xóa danh mục: " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}