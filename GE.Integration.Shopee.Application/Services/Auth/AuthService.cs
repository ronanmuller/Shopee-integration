using GE.Integration.Shopee.Application.Services.Cache;
using GE.Integration.Shopee.Domain.Response.Auth;
using GE.Integration.Shopee.Infra.External.Auth;

namespace GE.Integration.Shopee.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthUsersShopeeExternal _authUsersShopeeExternal;
        private readonly ICacheService _cacheService;

        public AuthService(IAuthUsersShopeeExternal authUsersShopeeExternal,
            ICacheService cacheService)
        {
            _authUsersShopeeExternal = authUsersShopeeExternal;
            _cacheService = cacheService;
        }

        public async Task<AuthUserResponse> GetToken(int shopId, string clientCode, CancellationToken cancellationToken)
        {
            var ret = await _authUsersShopeeExternal.GetToken(shopId, clientCode, cancellationToken);
            return ret;
        }

        public async Task<AuthUserResponse?> GetRefreshToken(int sellerId, string refreshToken, CancellationToken cancellationToken)
        {
            return await _authUsersShopeeExternal.GetRefreshToken(sellerId, refreshToken, cancellationToken);
        }
    }
}
