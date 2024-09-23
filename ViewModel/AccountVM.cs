using System.ComponentModel.DataAnnotations;

namespace ComputerDeviceShopping.ViewModel
{
    public class AccountVM
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string username { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải trên 6 ký tự và dưới 50 ký tự")]
        public string password { set; get; }
        public string firstName { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string lastName { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng email")]
        public string email { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string phone { set; get; }
        public string address { set; get; }
        public bool gender { set; get; }

    }
}
