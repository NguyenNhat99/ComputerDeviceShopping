namespace ComputerDeviceShopping.ViewModel
{
    public class ShoppingCartItem
    {
        public string IDProduct { set; get; }
        public string NameProduct { set; get; }
        public string ImageProduct { set; get; }
        public double Price { set; get; }
        public int Quantity { set; get; }
        public double Total { set; get; }
    }   
}
