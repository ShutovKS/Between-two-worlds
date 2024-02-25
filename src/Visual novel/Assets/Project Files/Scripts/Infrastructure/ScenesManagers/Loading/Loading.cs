#region

using System;
using System.Threading.Tasks;
using Data.Constant;
using Data.Localization.Dialogues;
using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using Tools.Camera;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Tools.Extensions.Resources;
#if YG_SERVICES
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
                LoadData();
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

#if YG_SERVICES
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

            await _uiFactory.CreatedImageCaptureForSaveScreen();
            _uiFactoryInfo.ImageCaptureForSaveUI.SetActivePanel(false);
            _localizerUI.Register(_uiFactoryInfo.ImageCaptureForSaveUI);
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

        private void LoadData()
        {
            var data = _saveLoadData.LoadOrCreateNew();

            var imageCaptureForSaveUI = _uiFactoryInfo.ImageCaptureForSaveUI;
            var backgroundUI = _uiFactoryInfo.ImageCaptureForSaveUI.BackgroundUI;
            var dialogueUI = _uiFactoryInfo.ImageCaptureForSaveUI.DialogueUI;

            imageCaptureForSaveUI.SetActivePanel(true);

            for (var index = 0; index < data.dialogues.Length; index++)
            {
                var dialoguesData = data.dialogues[index];

                if (dialoguesData.isDataExist == false)
                {
                    continue;
                }

                var phraseId = _localisationDataLoad.GetPhraseId(dialoguesData.idLastDialogue);

                switch (phraseId)
                {
                    case Phrase phrase:
                    {
                        backgroundUI.SetBackgroundImage(GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
                        dialogueUI.Answers.SetActiveAnswerOptions(false);
                        dialogueUI.DialogueText.SetAuthorName(phrase.Name);
                        dialogueUI.DialogueText.SetText(phrase.Text);
                        if (phrase.CharacterAvatarPath == null)
                        {
                            dialogueUI.Person.SetActionAvatar(false);
                        }
                        else
                        {
                            dialogueUI.Person.SetActionAvatar(true);
                            dialogueUI.Person.SetAvatar(GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
                        }

                        break;
                    }
                    case Responses response:
                    {
                        dialogueUI.Answers.SetActiveAnswerOptions(true);

                        var tuples = new (string, UnityAction)[response.ResponseList.Length];
                        for (var i = 0; i < response.ResponseList.Length; i++)
                        {
                            tuples[i] = (response.ResponseList[i].AnswerText, null);
                        }

                        dialogueUI.Answers.SetAnswerOptions(tuples);
                        break;
                    }
                }

                var texture2D = CameraTextureCapture.CaptureCameraView(backgroundUI.GetComponent<Canvas>(),
                    dialogueUI.GetComponent<Canvas>());

                dialoguesData.Background = texture2D;
            }

            imageCaptureForSaveUI.SetActivePanel(false);
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