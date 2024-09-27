using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]

    public class ProductsController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private static bool check = false;
        public IActionResult Index(int page = 1, string name = "")
        {
            check = false;
            paginition(page, name);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để chi tiết sản phẩm
        /// </summary>
        /// <param name="id">id sản phẩm cần chi tiết</param>
        /// <returns>Nếu có sản phẩm cần trả về thì nó sẽ trả về VIEW INSERT KÈM THEO SẢN PHÂM còn nếu không có gì trả về thì nó chuyển hướng đề lại view index</returns>
        public IActionResult Detail(string id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                UpdateInterface(product);
                ViewData["Spectifications"] = _context.Specifications.Where(d=>d.ProductId.Equals(product.ProductId)).ToList();
                return View("Insert", product);
            }
            return Redirect("Index");
        }
        public IActionResult InsertTestNew()
        {
            UpdateInterface(null);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InsertTestNew(Product pro, IFormFile avatar, IEnumerable<IFormFile> images, List<SpectificationVM> specList)
        {
            if (pro.ProductId == null)
            {
                Product product = new Product()
                {
                    ProductId = CommonTools.RandomNumbers(9),
                    ProductName = pro.ProductName,
                    ProductDescription = pro.ProductDescription,
                    DescriptionSummary = pro.DescriptionSummary,
                    Price = pro.Price,
                    Stock = pro.Stock,
                    BrandId = pro.BrandId,
                    CategoryId = pro.CategoryId,
                };
                product.Avatar = await CommonTools.SaveImage(avatar, "images", "products");

                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        Image img = new Image()
                        {
                            ProductId = product.ProductId,
                            ImageUrl = await CommonTools.SaveImage(image, "images", "products")
                        };
                        _context.Images.Add(img);
                    }
                }
                if (specList != null)
                {
                    foreach(var spec in specList)
                    {
                        var spectification = new Specification()
                        {
                            ProductId = product.ProductId,
                            SpecificationLabel = spec.label,
                            SpecificationDetail = spec.description,
                        };
                        _context.Specifications.Add(spectification);
                    }
                }
                _context.Products.Add(product);
                _context.SaveChanges();
                return Redirect("Index");
            }
            else
            {
                var product = _context.Products.FirstOrDefault(d => d.ProductId.Equals(pro.ProductId));
                if (product != null)
                {
                    product.ProductName = pro.ProductName;
                    product.ProductDescription = pro.ProductDescription;
                    product.DescriptionSummary = pro.DescriptionSummary;
                    product.Price = pro.Price;
                    product.Stock = pro.Stock;
                    product.BrandId = pro.BrandId;
                    if (avatar != null)
                    {
                        product.Avatar = await CommonTools.SaveImage(avatar, "images", "products");
                    }
                    if (images != null && images.Any())
                    {
                        var imagesProduct = _context.Images.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                        if (imagesProduct.Count() > 0)
                        {
                            foreach (var image in imagesProduct)
                            {
                                CommonTools.DeleteImageFromDirectory(image.ImageUrl);
                                _context.Images.Remove(image);
                            }
                        }
                        foreach (var image in images)
                        {
                            Image img = new Image()
                            {
                                ProductId = product.ProductId,
                                ImageUrl = await CommonTools.SaveImage(image, "images", "products")
                            };
                            _context.Images.Add(img);
                        }
                    }
                }
                _context.SaveChanges();
                return Redirect("Index");
            }
        }
        
        public IActionResult Insert()
        {
            UpdateInterface(null);
            return View(); 
        }
        /// <summary>
        /// Hàm này dùng để thêm hoăc cập nhật thông tin sản phẩm
        /// </summary>
        /// <param name="pro">Sản phẩm cần thêm hoặc sửa</param>
        /// <param name="avatar">Hình ảnh đại diện của sản phẩm</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Insert(Product pro, IFormFile avatar, IEnumerable<IFormFile> images,List<SpectificationVM> spectificationList)
        {
            if (pro.ProductId == null)
            {
                Product product = new Product()
                {
                    ProductId = CommonTools.RandomNumbers(9),
                    ProductName = pro.ProductName,
                    ProductDescription = pro.ProductDescription,
                    DescriptionSummary = pro.DescriptionSummary,
                    Price = pro.Price,
                    Stock = pro.Stock,
                    BrandId = pro.BrandId,  
                    CategoryId = pro.CategoryId,    
                };
                product.Avatar = await CommonTools.SaveImage(avatar, "images", "products");

                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        Image img = new Image()
                        {
                             ProductId = product.ProductId,
                             ImageUrl = await CommonTools.SaveImage(image, "images", "products")
                        };
                        _context.Images.Add(img);   
                    }
                }
                if(spectificationList != null)
                {
                    foreach(var spectification in spectificationList)
                    {
                        var s = new Specification()
                        {
                            SpecificationDetail = spectification.description,
                            SpecificationLabel = spectification.label,
                            ProductId = product.ProductId,
                        };
                        _context.Specifications.Add(s); 
                    }
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Redirect("Index");
            }
            else
            {
                var product = _context.Products.FirstOrDefault(d=> d.ProductId.Equals(pro.ProductId));
                if (product != null)
                {
                    product.ProductName = pro.ProductName;
                    product.ProductDescription = pro.ProductDescription;
                    product.DescriptionSummary = pro.DescriptionSummary;    
                    product.Price = pro.Price;
                    product.Stock = pro.Stock;
                    product.BrandId = pro.BrandId;
                    if (avatar != null)
                    {
                        product.Avatar = await CommonTools.SaveImage(avatar, "images", "products");
                    }
                    if (images != null && images.Any())
                    {
                        var imagesProduct = _context.Images.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                        if (imagesProduct.Count() > 0)
                        {
                            foreach(var image in imagesProduct)
                            {
                                CommonTools.DeleteImageFromDirectory(image.ImageUrl);
                                _context.Images.Remove(image);
                            }
                        }
                        foreach (var image in images)
                        {
                            Image img = new Image()
                            {
                                ProductId = product.ProductId,
                                ImageUrl = await CommonTools.SaveImage(image, "images", "products")
                            };
                            _context.Images.Add(img);
                        }
                    }
                    if (spectificationList != null)
                    {
                        RemoveSpectification(product.ProductId);
                        foreach (var spectification in spectificationList)
                        {
                            var s = new Specification()
                            {
                                SpecificationDetail = spectification.description,
                                SpecificationLabel = spectification.label,
                                ProductId = product.ProductId,
                            };
                            _context.Specifications.Add(s);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return Redirect("Index");
            }
        }
        /// <summary>
        /// Hàm này dùng để xóa tất cả các thông số kỹ thuật cũ của sản phẩm khi sửa
        /// </summary>
        /// <param name="idProduct">id của sản phẩm</param>
        private void RemoveSpectification(string idProduct)
        {
            var specticationList = _context.Specifications.Where(d => d.ProductId.Equals(idProduct)).ToList();
            foreach(var spectification in specticationList)
            {
                _context.Specifications.Remove(spectification);
            }
            _context.SaveChanges();
        }
        /// <summary>
        /// Hàm này dùng để xóa một sản phẩm
        /// </summary>
        /// <param name="id">id sản phẩm cần xóa</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/products/delete/{id}")]
        public IActionResult Delete(string id)
        {
            var product = _context.Products.Find(id);
            if (product != null) {
                var images = _context.Images.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                var favouriteList = _context.FavouriteLists.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                var comments = _context.Comments.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                RemoveSpectification(product.ProductId);
                foreach (var image in images) 
                {
                    CommonTools.DeleteImageFromDirectory(image.ImageUrl);
                    _context.Images.Remove(image);
                }
                foreach(var favourite in favouriteList)
                {
                    _context.FavouriteLists.Remove(favourite);
                }
                foreach(var cm in comments)
                {
                    _context.Comments.Remove(cm);
                }
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để thay đổi trạng thái CÒN HÀNG hoặc HẾT HÀNG
        /// </summary>
        /// <param name="id">id sản phẩm cần thay đổi</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/products/active/{id}")]
        public IActionResult Active(string id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.Stock = !product.Stock;
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để cập nhật lại giao diện
        /// Cập nhật lại các dropdown, select, hình ảnh khi xem chi tiết sản phẩm
        /// </summary>
        /// <param name="pro">Sản phẩm cần lấy các thông tin để hiển thị ra view chi tiết</param>
        private void UpdateInterface(Product pro)
        {
            if (pro != null)
            {
                //Trạng thái con hàng hoăc hết hàng để phục vụ cho việc sử dụng lại view insert để không bị lỗi ở <select></select>
                ViewBag.Stock = pro.Stock;
                ///Lấy ra id loại sản phẩm của một sản phẩm khi xem chi tiết
                ViewBag.CategorySelected = pro.CategoryId;
                //Đường dẫn url của hình ảnh đại diện sản phẩm
                ViewBag.AvatarProduct = pro.Avatar;
                //Danh sách hình ảnh của sản phẩm
                ViewBag.ImageProduct = _context.Images.Where(data => data.ProductId.Equals(pro.ProductId)).Select(data => data.ImageUrl).ToList();
                //Thương hiệu của sản phẩm
                ViewBag.BrandSelected = pro.BrandId;
            }
            ViewData["Brands"] = _context.Brands.ToList();
            ViewData["Categories"] = _context.Categories.ToList();
        }

        private void paginition(int page = 1, string name = "")
        {
            List<Product> products = new List<Product>();
            if (check)
            {
                products = _context.Products.Where(d =>String.IsNullOrEmpty(name) || d.ProductName.Contains(name)).ToList();
            }
            else {
                products = _context.Products.Where(d =>(d.UserId.Equals(UserLogged().UserId))&& String.IsNullOrEmpty(name) || d.ProductName.Contains(name)).ToList();
            }
            var pagVM = CommonTools.Paginition(products, page, 10);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Products"] = pagVM.data;
        }
        private Account UserLogged()
        {
            var userSession = HttpContext.Session.GetString("UserLogged");
            var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(userSession);
            return account;
        }
        public IActionResult DownloadFileExample()
        {

            var sampleData = "Name,Description,Price,Quantity\n" +
                        "Sample Product 1,Description 1,10.00,100\n" +
                        "Sample Product 2,Description 2,20.00,200\n";
            var bytes = Encoding.UTF8.GetBytes(sampleData);
            var output = new MemoryStream(bytes);

            return File(output, "text/csv", "sample_products.csv");
        }

        [CustomAuthorize("quản trị")]
        public IActionResult ProductsManagement(int page = 1, string name="")
        {
            check = true;
            paginition(page, name);
            return View();
        }
    }
}
