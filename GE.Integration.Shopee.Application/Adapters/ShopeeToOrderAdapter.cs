using GE.Contracts.DomainModels.Interfaces;
using GE.Contracts.DomainModels.Orders;
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

        public ICollection<OrderItem> OrderItems()
        {
            var orderItemList = new List<OrderItem>();

            foreach (var item in _orderItem.item_list)
            {
                var newOrderItem = new OrderItem
                {
                    OrderID = _orderItem.order_sn,
                    SKU = item.item_sku,
                    ItemId = item.item_id != null ? item.item_id.ToString() : string.Empty,
                    SoldAmount = item.model_quantity_purchased,
                    SoldValue = item.model_discounted_price != null ? Convert.ToDecimal(item.model_discounted_price) : 0,
                    ProductName = item.item_name + " - " + item.model_name
                };

                orderItemList.Add(newOrderItem);
            }

            return orderItemList;
        }

        public Payment Payment()
        {
           
            DateTime? payTime = null;
            if (!string.IsNullOrEmpty(_orderItem.pay_time))
                payTime = UnixTimeToDateTime(Convert.ToInt64(_orderItem.pay_time));

            var payment = new Payment
            {
                PaymentForm = _escrowDetail.response.order_income.buyer_payment_method,
                PaymentMethod = _escrowDetail.response.order_income.buyer_payment_method,
                PaymentDate = payTime,
                Installments = null,
                PaymentInternalId = _orderItem.order_sn,
                OrderId = _orderItem.order_sn,
                RebateCommissionValue = _escrowDetail.response.order_income.shopee_discount,
                AllCommissionValue = GetAllComissionValue
                    (_escrowDetail.response.order_income.seller_transaction_fee + 
                     _escrowDetail.response.order_income.service_fee + _escrowDetail.response.order_income.commission_fee),
                EcommerceNumber = _orderItem.order_sn

        };

            return payment;
        }

        public Shipping Shipping()
        {
            var shipping = new Shipping
            {
                State = _orderItem.recipient_address.state,
                City = _orderItem.recipient_address.city,
                ShippingCompany = _orderItem.shipping_carrier,
                ShippingMethod = _orderItem.shipping_carrier,
                ShippingSent = _orderItem.shipping_carrier,
                ShippingBy = _orderItem.shipping_carrier,
                TrackingCode = _orderItem.package_list == null ||
                               !_orderItem.package_list.Any() ? string.Empty : _orderItem.package_list[0].package_number,
                ShippingValuePaidOut = _escrowDetail.response.order_income.buyer_paid_shipping_fee.ToString(),
                ShippingInternalCode = _orderItem.package_list == null ||
                                       !_orderItem.package_list.Any() ? string.Empty : _orderItem.package_list[0].package_number,
                OrderId = _orderItem.order_sn,
                ShippingType = _orderItem.shipping_carrier,
                ShippingInternalType = _orderItem.fulfillment_flag,
                ShippingPaidStore = _orderItem.actual_shipping_fee,
                ShippingRebate = _escrowDetail.response.order_income.shopee_shipping_rebate,
                ShippingValuePaidClient = _escrowDetail.response.order_income.buyer_paid_shipping_fee,
                EcommerceNumber = _orderItem.order_sn
        };

            return shipping;
        }

        public Client OrderClient()
        {
            var client = new Client
            {
                Name = _orderItem.recipient_address.name,
                State = _orderItem.recipient_address.state,
                City = _orderItem.recipient_address.city,
                Document = _orderItem.buyer_cpf_id,
                OrderId = _orderItem.order_sn,
                Phone = _orderItem.recipient_address.phone, 
            };

            return client;
        }

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
