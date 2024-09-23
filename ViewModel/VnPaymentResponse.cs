using ComputerDeviceShopping.Models;

namespace ComputerDeviceShopping.ViewModel
{
    public class VnPaymentResponse
    {
        public bool Success { get; set; }
        public string PaymentMethods { set; get; }
        public string OrderDescription { set; get; }
        public string OrderId { set; get; }
        public string PaymentId { set; get; }
        public string TransactionId { set; get; }
        public string Token { set; get; }
        public string VnpayResponseCode { set; get; }

    }
  
}

