using System.Threading.Tasks;

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