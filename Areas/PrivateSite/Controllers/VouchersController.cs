using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using OfficeOpenXml;
using System.Data;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]
    public class VouchersController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index(int page = 1, string name = "")
        {
            paginition(page, name);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để thêm hoặc cập nhật thông tin cuả voucher
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Insert(Voucher v)
        {
            if (v.VoucherId == 0)
            {
                Voucher voucher = new Voucher()
                {
                    CreateAt = DateTime.Now,
                    VoucherCode = v.VoucherCode,
                    Discount = v.Discount,
                    VoucherStatus = true,
                    NumberOfTimesEXE = v.NumberOfTimesEXE, 
                    EndAt = v.EndAt
                };
                _context.Vouchers.Add(voucher);
                _context.SaveChanges();
            }
            else
            {
                var voucher = _context.Vouchers.Find(v.VoucherId);
                if (voucher != null)
                {
                    voucher.VoucherCode = v.VoucherCode;
                    voucher.Discount = v.Discount;
                    voucher.EndAt = v.EndAt;
                    voucher.NumberOfTimesEXE = v.NumberOfTimesEXE;  
                    ViewBag.Notification = "Đã sửa thành công";
                    _context.SaveChanges();
                }
            }
            return Redirect("index");
        }
        /// <summary>
        /// Hàm này hiển thị giao diện thêm nhiều voucher bằng file
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MultiInsert()
        {
            return View();
        }
        /// <summary>
        /// Hàm này dùng để load các voucher từ file mà người thêm đã thực hiên
        /// Giúp cho người quản trị có thể dễ dàng xem được danh sách các voucher mà mình sắp thêm
        /// </summary>
        /// <param name="file">file excel</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vouchers/loadmultiinsert")]
        public IActionResult LoadMultiInsert(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                List<Voucher> voucherList = new List<Voucher>();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    //ws đầu tiên
                    var worksheet = package.Workbook.Worksheets[0]; 
                    var rowCount = worksheet.Dimension.Rows;

                    //lý do cho row = 2 là bỏ qua hàng tiêu đề
                    for (int row = 2; row <= rowCount; row++) 
                    {
                        var voucherCode = worksheet.Cells[row, 1].Text; 
                        var discount = worksheet.Cells[row, 2].Text ?? ""; 
                        var day = worksheet.Cells[row, 3].Text;
                        var numberOfTimes = worksheet.Cells[row, 4].Text;

                        if (!string.IsNullOrEmpty(voucherCode) && !string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(discount))
                        {
                            var vc = _context.Vouchers.FirstOrDefault(p => p.VoucherCode.Equals(voucherCode));
                            if (vc != null)
                                return Json(new { success = false, message = "Mã voucher: " + voucherCode + " đã có trong danh sách" });
                            var voucher = new Voucher
                            {
                                VoucherCode = voucherCode,
                                Discount = (discount != null ? int.Parse(discount) : 0),
                                CreateAt = DateTime.Now,
                                EndAt = DateTime.Now.AddDays(int.Parse(day)),
                                VoucherStatus = true,
                                NumberOfTimesEXE = int.Parse(numberOfTimes)
                            };
                            voucherList.Add(voucher);
                        }
                        else
                        {
                            return Json(new { success = false, message = "Dữ liệu không hợp lệ tại hàng hoặc thiếu" });
                        }
                    }
                }
                return Json(new { success = true, message = "Đã thêm thành công", data = voucherList });
            }
            return Json(new { success = false, message = "File không hợp lệ hoặc không có file nào được tải lên" });
        }
        /// <summary>
        /// Hàm này dùng để xác nhận thêm danh sách voucher 
        /// </summary>
        /// <param name="vouchers">Danh sách voucher cần thêm</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/vouchers/insertmulticonfirm")]
        public IActionResult ComfirmMultiInsert([FromBody] List<Voucher> vouchers)
        {
            if (vouchers != null)
            {
                foreach (var voucher in vouchers)
                {
                    Voucher v = new Voucher()
                    {
                        CreateAt = DateTime.Now,
                        Discount = voucher.Discount,
                        VoucherCode = voucher.VoucherCode,
                        VoucherStatus = voucher.VoucherStatus,
                        EndAt = (DateTime)voucher.EndAt,
                        NumberOfTimesEXE = voucher.NumberOfTimesEXE
                    };
                    _context.Vouchers.Add(v);
                }
                _context.SaveChanges();
                return Json(new { success = true, message = "Đã thêm thành công" });
            }
            return Json(new { success = false, message = "Đã thêm thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để cập nhật thông tin voucher
        /// </summary>
        /// <param name="id">Mã voucher cần cập nhật</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/vouchers/update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(d => d.VoucherId == id);
            if (voucher == null)
                return NotFound();

            return Json(new { success = true, data = new { voucher.VoucherId, voucher.Discount, voucher.VoucherCode, voucher.VoucherStatus, ca = voucher.CreateAt.HasValue ? voucher.CreateAt.Value.ToString("MM/dd/yyyy HH:mm:ss") : null, voucher.EndAt }, message = "Lấy dữ liệu thành công" });
        }
        /// <summary>
        /// Hàm này dùng để xóa một voucher
        /// </summary>
        /// <param name="id">Số thứ tự voucher cần xóa</param>
        /// <returns>Một json bao gồm: trạng thái và lời nhắn</returns>
        [HttpDelete]
        [Route("api/vouchers/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa xóa thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để tạo random mã voucher nhanh
        /// </summary>
        /// <returns>Một json gòm: mã random, trạng thái và lời nhắn</returns>
        [HttpGet]
        [Route("api/vouchers/randomcode")]
        public IActionResult RandomCodeVoucher()
        {
            string codeRamdom = CommonTools.RandomCharacters(11);
            if (codeRamdom != null)
                return Json(new { success = true, code = codeRamdom, message = "Xóa thành công" });
            return Json(new { success = false, message = "Xóa xóa thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để bật tắt hoạt động của một voucher
        /// </summary>
        /// <param name="id">id của voucher cần thay đổi trạng thái</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/vouchers/active/{id}")]
        public IActionResult Active(int id)
        {
            var voucher = _context.Vouchers.Find(id);
            if (voucher != null)
            {
                voucher.VoucherStatus = !voucher.VoucherStatus;
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để phân trang và tìm kiếm
        /// </summary>
        /// <param name="page">trang</param>
        /// <param name="name">tìm kiếm bằng mã voucher</param>
        private void paginition(int page = 1, string name = "")
        {
            List<Voucher> vouchers = _context.Vouchers.ToList();
            var pagVM = CommonTools.Paginition(vouchers, page, 10);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Vouchers"] = pagVM.data;
        }
    }
}
