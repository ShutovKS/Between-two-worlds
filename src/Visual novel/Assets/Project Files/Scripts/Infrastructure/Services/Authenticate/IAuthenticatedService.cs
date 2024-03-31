using System.Threading.Tasks;

namespace Infrastructure.Services.Authenticate
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated { get; }

        Task<bool> Login();
    }
}