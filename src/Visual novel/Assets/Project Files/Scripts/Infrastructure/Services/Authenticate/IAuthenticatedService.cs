namespace Infrastructure.Services.Authenticate
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated { get; }

        bool Login();
    }
}