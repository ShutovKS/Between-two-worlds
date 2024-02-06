#region

using System;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;
#if YG_SERVICES && UNITY_WEBGL
using Data.Constant;
using YG;
#endif

// using YG;

#endregion

namespace Infrastructure.ScenesManagers.Loading
{
    public class Loading : MonoBehaviour
    {
        private string _language = "";
        private IAssetsAddressablesProviderService _assetsAddressablesProvider;
        private ILocalisationDataLoadService _localisationDataLoad;
        private ICoroutineRunnerService _coroutineRunner;
        private ILocalizerUIService _localizerUI;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryService _uiFactory;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ISoundsService _sounds;

        private async void Start()
        {
            await ServicesInitialize();
            await CreatedUI();
            LanguageSelected(() =>
            {
                LocalisationUI();
                OpenMainMenu();
            });
        }

        private async Task ServicesInitialize()
        {
            _assetsAddressablesProvider = new AssetsAddressablesProviderService();
            _localisationDataLoad = new LocalisationDataLoadService();
            _localizerUI = new LocalizerUIServiceService();
            _uiFactory = new UIFactoryService(_assetsAddressablesProvider);
            _uiFactoryInfo = _uiFactory;
            _coroutineRunner = new GameObject().AddComponent<CoroutineRunnerServiceService>();
            _sounds = new SoundsService();

#if YG_SERVICES && UNITY_WEBGL
            _saveLoadData = new SaveLoadDataYGService();
            await InitializeYandexGameSDK();
#else
            _saveLoadData = new SaveLoadDataLocalService();
#endif
            ServicesContainer.SetServices(
                _assetsAddressablesProvider,
                _saveLoadData,
                _uiFactory,
                _localisationDataLoad,
                _localizerUI,
                _coroutineRunner,
                _sounds);
        }

        private async Task CreatedUI()
        {
            await _uiFactory.CreatedBackgroundScreen();
            _uiFactoryInfo.BackgroundUI.SetBackgroundColor(Color.black);

            await _uiFactory.CreatedChooseLanguageScreen();
            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(true);

            await _uiFactory.CreatedDialogueScreen();
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.DialogueUI);

            await _uiFactory.CreatedMainMenuScreen();
            _uiFactoryInfo.MainMenuUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.MainMenuUI);

            await _uiFactory.CreatedConfirmationScreen();
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.ConfirmationUI);

            await _uiFactory.CreatedSaveLoadScreen();
            _uiFactoryInfo.SaveLoadUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.SaveLoadUI);

            await _uiFactory.CreatedLastWordsScreen();
            _uiFactoryInfo.LastWordsUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.LastWordsUI);
        }

        private void LanguageSelected(Action onCompleted)
        {
            var localizationsInfo = _localisationDataLoad.GetLocalizationsInfo();

            foreach (var localizationInfo in localizationsInfo)
            {
                _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.AddLanguageInScrollView(
                    localizationInfo.Language,
                    localizationInfo.FlagImage,
                    () =>
                    {
                        _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(false);
                        _language = localizationInfo.Language;
                        _localisationDataLoad.Load(_language);
                        onCompleted?.Invoke();
                    });
            }
        }

        private void LocalisationUI()
        {
            var uiLocalisation = _localisationDataLoad.GetUILocalisation();
            _localizerUI.Localize(uiLocalisation);
        }

        private void OpenMainMenu()
        {
            SceneManager.LoadScene("2.Meta");
        }

        #region SDK

#if YG_SERVICES
        private async Task InitializeYandexGameSDK()
        {
            
            var path = AssetsAddressablesPath.YANDEX_GAME_PREFAB;
            var prefab = await _assetsAddressablesProvider.GetAsset<GameObject>(path);
            
            var instance = Instantiate(prefab);
            DontDestroyOnLoad(instance);

            var isInitialized = false;
            var yandexGame = instance.GetComponent<YandexGame>();
            
            yandexGame.RejectedAuthorization.AddListener(OnInitialized);
            yandexGame.ResolvedAuthorization.AddListener(OnInitialized);
            
            Debug.Log("Yandex Game SDK load initializing");
            
            instance.SetActive(true);
            
            while (!isInitialized)
            {
                await Task.Yield();
            }
            
            yandexGame.RejectedAuthorization.RemoveListener(OnInitialized);
            yandexGame.ResolvedAuthorization.RemoveListener(OnInitialized);

            YandexMetrica.Send("started");

            return;

            void OnInitialized()
            {
                isInitialized = true;
            }
        }
#endif

        #endregion
    }
}