using GE.Integration.Shopee.Domain.Response.Payment;

namespace GE.Integration.Shopee.Infra.External.Payment
{
    public interface IGetEscrowDetailsExternal
    {
        Task<EscrowDetailResponse> GetEscrowDetails(string accessToken, int shopId, string order_sn);
    }
}
