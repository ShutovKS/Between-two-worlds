using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Infrastructure.Services.Authenticate
{
    public class AuthenticatedGooglePlayService : IAuthenticateService
    {
        public bool IsAuthenticated => Social.localUser.authenticated;

        public async Task<bool> Login()
        {
            var result = false;
            var isResult = false;

            PlayGamesPlatform.Instance.Authenticate(signInStatus =>
            {
                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        Debug.Log("Login with Google Play games successful.");
                        Social.localUser.Authenticate(successAuth => result = successAuth);
                        break;
                    case SignInStatus.InternalError:
                        Debug.Log("Login social user unsuccessful");
                        result = false;
                        break;
                    case SignInStatus.Canceled:
                        Debug.Log("Login Unsuccessful");
                        result = false;
                        break;
                    default:
                        result = false;
                        break;
                }

                isResult = true;
            });

            var waitTime = 5f;
            while (isResult == false && waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                await Task.Yield();
            }


            return isResult && result;
        }
    }
}