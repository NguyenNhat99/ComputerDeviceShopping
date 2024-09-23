namespace ComputerDeviceShopping.ViewModel
{
    public class PaginitionVM<T>
    {
        
        public int page { set; get; }
        public int noOfPages { set; get; }

        public int displayPage { set; get; }
        public List<T> data { set; get; }
      
        public PaginitionVM(int _page, int _noOfPages, int _displayPage, List<T> _data) 
        {
            page = _page;
            noOfPages = _noOfPages;
            displayPage = _displayPage;
            data = _data;
        }
    }
}
