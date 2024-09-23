using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ComputerDeviceShopping.ViewModel
{
    public class ArticleVM
    {
        [AllowNull]
        public int id { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string name { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]
        public string content { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống")]

        public string summaryContent { set; get; }
        [AllowNull]
        public IFormFile avatar { set; get; }
    }
}
