using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using System.Diagnostics.Eventing.Reader;
using System.Text;
namespace ComputerDeviceShopping.Common
{
    
    public class CommonTools
    {
        /// <summary>
        /// Hàm này dùng để random số ngẫu nhiên
        /// </summary>
        /// <param name="n">Số lượng số cần random</param>
        /// <returns>Một chuỗi số random</returns>
        public static string RandomNumbers(int n)
        {
            string result = "";
            Random random = new Random();
            int number;
            for (int i = 0; i < n; i++)
            {
                number = random.Next(9);
                if (number == 0 && i == 0)
                    number = 1;
                result += number;
            }
            return result;
        }
        /// <summary>
        /// Hàm này dùng để random các ký tự
        /// </summary>
        /// <param name="n">Số ký tự sẽ random</param>
        /// <returns>Một chuỗi ký tự đã random</returns>
        public static string RandomCharacters(int n)
        {
            StringBuilder result = new StringBuilder();
            Random random = new Random();
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            for (int i = 0; i < n; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
        /// <summary>
        /// Hàm này dùng để phân trang
        /// </summary>
        /// <typeparam name="T">Là một đối tượng </typeparam>
        /// <param name="objects">Đối tượng như danh sách: tài khoản, bài viết,...</param>
        /// <param name="page">Trang hiện tại đang xem</param>
        /// <param name="ndisplay">Số trang được hiển thị</param>
        /// <returns></returns>
        public static PaginitionVM<T> Paginition<T>(List<T> objects, int page, int ndisplay)
        {
            int NoOfProductOnPage = ndisplay;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(objects.Count) / Convert.ToDouble(NoOfProductOnPage)));
            int SkipPageNumber = (page - 1) * NoOfProductOnPage;

            int _NoOfPages = ((page >= 5) ? ((page + 4 > NoOfPages) ? NoOfPages : (page + 4)) : (page >= 5 ? 5 : NoOfPages));
            int _DisplayPage = (page < 5 ? 0 : (((page - 1) >= (NoOfPages - 5)) ? (NoOfPages - 5) : (page - 1)));

            objects = objects.Skip(SkipPageNumber).Take(NoOfProductOnPage).ToList();
            PaginitionVM<T> pvm = new PaginitionVM<T>(page, _NoOfPages, _DisplayPage, objects);

            return pvm;
        }
        /// <summary>
        /// Hàm này sẽ dùng để lưu hình ảnh
        /// </summary>
        /// <param name="image">hình ảnh cần lưu</param>
        /// <param name="webHostEnvironment">Cung cấp thông tin về môi trường chạy của ứng dụng web, gồm dẫn tới thư mục root của ứng dụng</param>
        /// <param name="path1">Đường dẫn con của thư mục Asset</param>
        /// <param name="path2">Đường dẫn con của path2</param>
        /// <returns>Trả về một đường dẫn sau khi lưu hình ảnh. Tiếp theo dùng đường dẫn này gán vào biến trong database</returns>
        public static async Task<string> SaveImage(IFormFile image, string path1, string path2)
        {
            string imageURL = "";
            if (image != null && image.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path1,path2);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(uploadsFolder,uniqueFileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
                imageURL = "/" + path1 + "/" + path2 + "/" + uniqueFileName;
                return imageURL;
            }

            return imageURL;
        }
        /// <summary>
        /// Hàm này dùng để xóa hình ảnh sản phẩm ra khỏi thư mục
        /// </summary>
        /// <param name="imageUrl">Đường dẫn hình ảnh cần xóa ra khỏi thư mục</param>
        public static void DeleteImageFromDirectory(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"+imageUrl);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
        public static async Task<string> SaveMultiImage(List<IFormFile> image, string path1, string path2)
        {
            int n_image = image.Count;
            string imageURL = "";
            if (n_image > 0)
            {
                for(int i = 0;i< n_image; i++)
                {
                    imageURL = "";
                    if (image[i] != null && image[i].Length > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path1, path2);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image[i].FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image[i].CopyToAsync(fileStream);
                            fileStream.Flush();
                        }
                        imageURL = "/" + path1 + "/" + path2 + "/" + uniqueFileName;
                        return imageURL;
                    }
                }
            }
            return imageURL;
        }
    }
}
