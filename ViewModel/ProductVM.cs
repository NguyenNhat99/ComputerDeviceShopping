namespace ComputerDeviceShopping.ViewModel
{
    public class ProductVM
    {
        public string id { set; get; }
        public string name { set; get; }
        public string descriptionSummary { set; get; }
        public string description { set; get; }
        public int price { set; get; }
        public IFormFile avatar { set; get; }
        public List<IFormFile> imagesProduct { set; get; }

    }
}
