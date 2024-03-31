using System.Threading.Tasks;
using Data.Constant;
using Infrastructure.Services.AssetsAddressables;
using UnityEngine;
using YG;

namespace Infrastructure.Services.Authenticate
{
    public class AuthenticatedStubService : IAuthenticateService
    {
        public bool IsAuthenticated => false;

        public Task<bool> Login()
        {
            return Task.FromResult(IsAuthenticated);
        }
    }
}