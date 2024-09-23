using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]

    public class BrandsController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index(int page = 1, string name = "")
        {
            paginition(page,name);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Insert(Brand b)
        {
            ViewBag.updateItem = "";
            var brand = await _context.Brands.FirstOrDefaultAsync(d => d.BrandId == b.BrandId);
            if (brand != null && b.BrandId != 0)
            {
                brand.BrandName = b.BrandName;
            }
            else
            {
                _context.Brands.Add(new Brand { BrandName = b.BrandName });
            }
            await _context.SaveChangesAsync();
            return Redirect("Index");
        }

        [HttpGet]
        [Route("api/brands/update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(d => d.BrandId == id);
            if (brand == null)
                return NotFound();
            return Json(new { success = true, data = brand, message = "Lấy dữ liệu thành công" });
        }
        /// <summary>
        /// Hàm này dùng để xóa loại sản phẩm
        /// </summary>
        /// <param name="id">ID của loại sản phẩm</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/brands/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new { success = false, message = "Xóa thất bại" });
            _context.Brands.Remove(brand);
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
            List<Brand> brands = _context.Brands.Where(d => String.IsNullOrEmpty(name) || (d.BrandName ?? "").Contains(name)).ToList();
            var pagVM = CommonTools.Paginition(brands, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Brands"] = pagVM.data;
        }
    }
}
