using ComputerDeviceShopping.Payments;
using ComputerDeviceShopping.ViewModel;

namespace ComputerDeviceShopping.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequest model);
        VnPaymentResponse PaymentExcute(IQueryCollection collection);
    }
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration config) 
        {
            _config = config;
        }
        /// <summary>
        /// Hàm này dùng để tạo link url vnpay thanh toans
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequest model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:vnp_TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount*100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreateAt.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng:" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:vnp_ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", model.OrderId);

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:vnp_Url"], _config["VnPay:vnp_HashSecret"] );
            return paymentUrl;
        }
        /// <summary>
        /// Hàm này dùng để thực thi và trả về một phản hồi
        /// </summary>
        /// <param name="collections"></param>
        /// <returns></returns>

        public VnPaymentResponse PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key,value) in collections) 
            {
                if(!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_orderid = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:vnp_HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponse()
                {
                    Success = false,
                };
            }
            else { 
                return new VnPaymentResponse 
                { Success = true, 
                    PaymentMethods = "VnPay",
                    OrderId = vnp_orderid.ToString(), 
                    OrderDescription = vnp_OrderInfo, 
                    TransactionId = vnp_TransactionId.ToString(),
                    Token = vnp_SecureHash,
                    VnpayResponseCode = vnp_ResponseCode,   
                };
            }
        }
    }
}
