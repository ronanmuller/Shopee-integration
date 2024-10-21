using GE.Integration.Shopee.Domain.Response.Auth;

namespace GE.Integration.Shopee.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthUserResponse> GetToken(int shopId, string clientCode, CancellationToken cancellationToken);
        Task<AuthUserResponse?> GetRefreshToken(int shopId, string refreshToken, CancellationToken cancellationToken);
    }
}
