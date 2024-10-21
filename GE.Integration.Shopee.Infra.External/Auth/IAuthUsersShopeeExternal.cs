using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Infra.External.Auth
{
    public interface IAuthUsersShopeeExternal
    {
        Task<string> GetAuthUrl(string externalID);

        Task<AuthUserResponse> GetToken(int shopId, string clientCode, CancellationToken cancellationToken);

        Task<AuthUserResponse> GetRefreshToken(int shopId, string refreshToken, CancellationToken cancellationToken);

    }
}
