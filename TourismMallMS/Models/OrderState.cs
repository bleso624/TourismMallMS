using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models
{
    public enum OrderState
    {
        Pending, // 订单已生成
        Processing, // 支付处理中
        Completed, // 交易成功
        Declined, // 交易失败
        Cancelled, // 订单取消
        Refund, // 已退款
    }
}
