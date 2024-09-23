namespace ComputerDeviceShopping.Common
{
    public static class ConvertHelper
    {
        public static string ConvertToVND(double amount)
        {
            return String.Format("{0:#,##0}", amount) + " VND";
        }
    }
}
