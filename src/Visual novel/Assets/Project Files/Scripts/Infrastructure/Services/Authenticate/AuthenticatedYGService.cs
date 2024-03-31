using System.Threading.Tasks;
using Data.Constant;
using Infrastructure.Services.AssetsAddressables;
using UnityEngine;
using YG;

namespace Infrastructure.Services.Authenticate
{
    public class AuthenticatedYGService : IAuthenticateService
    {
        public AuthenticatedYGService(IAssetsAddressablesProviderService assetsAddressablesProviderService)
        {
            _assetsAddressablesProviderService = assetsAddressablesProviderService;
        }

        public bool IsAuthenticated => YandexGame.auth;

        private readonly IAssetsAddressablesProviderService _assetsAddressablesProviderService;

        public async Task<bool> Login()
        {
            await InitializeYandexGameSDK();

            return IsAuthenticated;
        }

        private async Task InitializeYandexGameSDK()
        {
            const string path = AssetsAddressablesPath.YANDEX_GAME_PREFAB;
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(path);

            var instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);

            var isInitialized = false;
            var yandexGame = instance.GetComponent<YandexGame>();

            yandexGame.RejectedAuthorization.AddListener(OnInitialized);

            Debug.Log("Yandex Game SDK load initializing");

            instance.SetActive(true);

            var waitTime = 5f;
            while (!isInitialized && waitTime <= 0)
            {
                waitTime -= Time.deltaTime;
                await Task.Yield();
            }

            yandexGame.RejectedAuthorization.RemoveListener(OnInitialized);

            return;

            void OnInitialized()
            {
                isInitialized = true;
            }
        }
    }
}