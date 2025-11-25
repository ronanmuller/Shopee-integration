using GE.Integration.Shopee.Domain.Response.Orders;
using GE.Integration.Shopee.Domain.Response.Payment;
using GE.Integration.Shopee.Domain.Response.Shop;

namespace GE.Integration.Shopee.Application.Adapters
{

    public class ShopeeToOrderAdapter : IOrderAdapter
    {
        private readonly Domain.Response.Orders.Order _orderItem;
        private readonly EscrowDetailResponse _escrowDetail;
        private readonly ShopInfoResponse _shopInfo;

        public ShopeeToOrderAdapter(Domain.Response.Orders.Order item, 
            EscrowDetailResponse escrowDetail, 
            ShopInfoResponse shopInfo)
        {
            _orderItem = item;
            _escrowDetail = escrowDetail;
            _shopInfo = shopInfo;
        }

        public string InternalId() => _orderItem.order_sn;

        public string EcommerceNumber() => _orderItem.order_sn;

        public string SalesChannel() => _shopInfo.shop_name;

        public DateTime? BillingDate()
        {
            if (!string.IsNullOrEmpty(_orderItem.pay_time))
                return UnixTimeToDateTime(Convert.ToInt64(_orderItem.pay_time));

            return null;
        }

        public DateTime? OrderDate()
        {
            if (!string.IsNullOrEmpty(_orderItem.create_time))
                return UnixTimeToDateTime(Convert.ToInt64(_orderItem.create_time));

            return null;
        }

        public string RemoteId() => _orderItem.order_sn;

        public string CartId() => _orderItem.order_sn;

        public string AccountNickName() => _shopInfo.shop_name;

        public DateTime? SoldDate()
        {
            if (!string.IsNullOrEmpty(_orderItem.create_time))
                return UnixTimeToDateTime(Convert.ToInt64(_orderItem.create_time));

            return null;
        }

        public DateTime? PaymentDate()
        {
            if (!string.IsNullOrEmpty(_orderItem.pay_time))
                return UnixTimeToDateTime(Convert.ToInt64(_orderItem.pay_time));

            return null;
        }

        public string OrderStatus() => _orderItem.order_status;

        public DateTime? CanceledDate()
        {
            if (OrderStatus() == "CANCELLED")
                return DateTime.Now;

            return null;
        }

        public decimal DescountValue()
        {
            if (_escrowDetail.response.order_income != null! && 
                _escrowDetail.response.order_income.order_discounted_price != null)
                return Convert.ToDecimal(_escrowDetail.response.order_income.order_discounted_price);

            return 0;
        }

        public decimal FreightCost()
        {
            if (_escrowDetail.response.order_income != null &&
                _escrowDetail.response.order_income.buyer_paid_shipping_fee != null)
                return Convert.ToDecimal(_escrowDetail.response.order_income.buyer_paid_shipping_fee);

            return 0;
        }

        public decimal TotalGrossValue() => _orderItem.total_amount == null ? 0 : Convert.ToDecimal(_orderItem.total_amount);

        public string FreightMode() => _orderItem.shipping_carrier;

        public string BookMarks() => string.Empty;


        #region Private methods

        private decimal GetAllComissionValue(decimal sellerTransactionFee = 0, decimal serviceFee = 0, decimal commissionFee = 0)
        {
            return sellerTransactionFee + commissionFee + serviceFee;
        }

        private static DateTime UnixTimeToDateTime(long unixTime)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTime);
            return dateTime;
        }

        #endregion
    }
}
