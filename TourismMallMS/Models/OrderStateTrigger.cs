namespace TourismMallMS.Models
{
    public enum OrderStateTrigger
    {
        PlaceOrder, // 支付
        Approve, // 收款成功
        Reject, // 收款失败
        Cancel, // 取消
        Return // 退货
    }
}