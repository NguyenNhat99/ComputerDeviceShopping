using System.ComponentModel.DataAnnotations;

namespace ComputerDeviceShopping.DTOs.Accounts
{
    public sealed class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; } = "";

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = "";

        [Required, MinLength(6)]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = "";

    }
}
