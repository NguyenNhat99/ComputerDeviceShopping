namespace ComputerDeviceShopping.ViewModel
{
    public class VnPaymentRequest
    {
        public string OrderId { set; get; }
        public string FullName { set; get; }
        public string Description { set; get; }
        public double Amount { set; get; }
        public DateTime CreateAt { set; get; }
        /// <summary>
        /// //////////////////////////////////////////
        /// </summary>


        //public string note { set; get; }
        //public string CustomerId { set; get; }
        //public string VoucherId { set; get; }
        //public int StatusId { set; get; }
    }
}
