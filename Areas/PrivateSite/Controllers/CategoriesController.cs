using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime.CompilerServices;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]
    public class CategoriesController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index(int page = 1, string name = "")
        {
            paginition(page, name);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để cập nhật lại loại sản phẩm
        /// </summary>
        /// <param name="ct">Loại sản phẩm cần thêm vào danh sách</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Insert(Category ct)
        {
            ViewBag.updateItem = "";
            var category = await _context.Categories.FirstOrDefaultAsync(d => d.CategoryId == ct.CategoryId);

            if (category != null)
            {
                category.CategoryName = ct.CategoryName;
                category.CategorySymbol = ct.CategorySymbol;
            }
            else
            {
                _context.Categories.Add(new Category { CategoryName = ct.CategoryName, CategorySymbol = ct.CategorySymbol });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult InsertAndUpdateMulti()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertAndUpdateMulti(IFormFile file) 
        {
            if (file != null && file.Length > 0)
            {
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Bỏ qua hàng tiêu đề
                    {
                        var categoryName = worksheet.Cells[row, 1].Text; // Giả sử cột đầu tiên là tên sản phẩm
                        var categorySymbol = worksheet.Cells[row, 2].Text; // Giả sử cột thứ hai là loại sản phẩm

                        var category = new Category
                        {
                          CategoryName = categoryName,
                          CategorySymbol = categorySymbol
                          
                        };
                        _context.Categories.Add(category);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Products imported successfully!";
                }
            }
            else
            {
                TempData["Error"] = "Please upload a file.";
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("api/categories/update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(d => d.CategoryId == id);
            if (category == null)
                return NotFound();
            return Json(new { success = true, data = category, message = "Lấy dữ liệu thành công" });
        }
        /// <summary>
        /// Hàm này dùng để xóa loại sản phẩm
        /// </summary>
        /// <param name="id">ID của loại sản phẩm</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/categories/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { success = false, message = "Xóa thất bại" });
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa thành công" });
        }
        /// <summary>
        /// Hàm này dùng để phân trang và tìm kiếm sản phẩm
        /// </summary>
        /// <param name="page">Số trang đang đứng</param>
        /// <param name="name">tên loại sản phẩm cần tìm kiếm</param>
        private void paginition(int page = 1, string name = "")
        {
            List<Category> categories = _context.Categories.Where(d => String.IsNullOrEmpty(name) || (d.CategoryName??"").Contains(name)).ToList();
            var pagVM = CommonTools.Paginition(categories, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Categories"] = pagVM.data;
        }
    }
}
