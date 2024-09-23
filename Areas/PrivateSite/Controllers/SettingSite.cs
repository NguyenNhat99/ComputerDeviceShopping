using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]

    public class SettingSite : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Slider(int page = 1, string name = "")
        {
            paginition(page, name);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Slider(Slider slide,IFormFile image)
        {
            if(slide.SlideId == 0)
            {
                Slider slider = new Slider()
                {
                    ProductName = slide.ProductName,
                    SliderTitle = slide.SliderTitle,    
                    ProductLink = slide.ProductLink,
                    DescriptionSlide = slide.DescriptionSlide,  
                };
                slider.SlideImage = await CommonTools.SaveImage(image, "images", "slide");
                _context.Sliders.Add(slider);
                _context.SaveChanges();
                return Redirect("Slider");
            }
            else
            {
                var slideUpdate = _context.Sliders.Find(slide.SlideId);
                if (slideUpdate != null)
                {
                    slideUpdate.SliderTitle = slide.SliderTitle;
                    slideUpdate.ProductLink = slide.ProductLink;
                    slideUpdate.ProductName = slide.ProductName;
                    slideUpdate.DescriptionSlide = slide.DescriptionSlide;
                    if (image != null)
                        slideUpdate.SlideImage = await CommonTools.SaveImage(image, "images", "slide");
                }
                _context.SaveChanges();
                return Redirect("Slider");
            }
        }
        [HttpGet]
        [Route("api/settingsite/updateslide/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var slide = await _context.Sliders.FirstOrDefaultAsync(d => d.SlideId == id);
            if (slide == null)
                return NotFound();
            ViewBag.SlideImage = slide.SlideImage;
            return Json(new { success = true, data = slide, message = "Lấy dữ liệu thành công" });
        }
        [HttpDelete]
        [Route("api/settingsiteslider/delete/{id}")]
        public async Task<IActionResult> DeleteSlider(int id)
        {
            var slide = await _context.Sliders.FindAsync(id);
            if (slide != null)
            {
                _context.Sliders.Remove(slide);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa xóa thất bại" });
        }
        public IActionResult UpdateSlider(int id)
        {
            var slide = _context.Sliders.Find(id);
            if (slide != null)
            {

            }
            return View();
        }
        private void paginition(int page = 1, string name = "")
        {
            List<Slider> categories = _context.Sliders.Where(d => String.IsNullOrEmpty(name) || (d.ProductName ?? "").Contains(name)).ToList();
            var pagVM = CommonTools.Paginition(categories, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Slider"] = pagVM.data;
        }
    }
}
