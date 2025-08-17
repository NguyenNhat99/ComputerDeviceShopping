using System.ComponentModel.DataAnnotations;

namespace ComputerDeviceShopping.DTOs.Accounts
{
    public sealed class ChangeInfoDTO
    {
        [Required] 
        public string FirstName { get; set; } = "";
        [Required] 
        public string LastName { get; set; } = "";
        public string? Phone { get; set; }
        public string? DeliverAddress { get; set; }
        public bool? Gender { get; set; }
    }
}
