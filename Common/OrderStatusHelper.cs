namespace ComputerDeviceShopping.Common
{
    public static class OrderStatusHelper
    {
        public static string GetBadgeClassById(int status)
        {

            switch (status)
            {
                case 5:
                    return "bg-success";
                case 4:
                case 6:
                    return "bg-danger";
                case 2:
                    return "bg-primary";
                default:
                    return "bg-secondary";
            }
        }
        public static string getNameStatusById(int status)
        {
            switch (status)
            {
                case 5:
                    return "Thành công";
                case 4:
                    return "Thất bại";
                case 6:
                    return "Đã hủy";
                case 2:
                    return "Đang vận chuyển";
                default:
                    return "Chờ xác nhận";
            }
        }
    }
}
