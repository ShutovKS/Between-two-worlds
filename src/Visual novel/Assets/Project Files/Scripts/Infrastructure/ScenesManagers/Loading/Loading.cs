#region

using System.Threading.Tasks;
using Data.Localization.Dialogues;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.UIFactory;
using Tools.Camera;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;
using static Tools.Extensions.Resources;

#endregion

namespace Infrastructure.ScenesManagers.Loading
{
    public class Loading : MonoBehaviour
    {
        [Inject]
        public void Construct(ILocalisationDataLoadService localisationDataLoad, IUIFactoryInfoService uiFactoryInfo,
            ILocalizerUIService localizerUI, IUIFactoryService uiFactory, IProgressService progress,
            IMetricService metric)
        {
            Debug.Log("Loading Construct");
            
            _localisationDataLoad = localisationDataLoad;
            _uiFactoryInfo = uiFactoryInfo;
            _localizerUI = localizerUI;
            _uiFactory = uiFactory;
            _progress = progress;
            _metric = metric;
        }

        private ILocalisationDataLoadService _localisationDataLoad;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ILocalizerUIService _localizerUI;
        private IUIFactoryService _uiFactory;
        private IProgressService _progress;
        private IMetricService _metric;

        private async void Start()
        {
            await CreatedUI();

            OpenLanguageSelectionMenu();

            _metric.SendEvent(MetricEventType.Started);
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

        private void OpenLanguageSelectionMenu()
        {
            var localizationsInfo = _localisationDataLoad.GetLocalizationsInfo();

            foreach (var localizationInfo in localizationsInfo)
            {
                _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.AddLanguageInScrollView(
                    localizationInfo.Language,
                    localizationInfo.FlagImage);
            }

            _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.OnSelectLanguage += SelectLanguage;
        }

        private void SelectLanguage(string language)
        {
            _localisationDataLoad.Load(language);

            _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.OnSelectLanguage -= SelectLanguage;

            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(false);

            LocalisationUI();
            LoadData();
            OpenMainMenu();
        }

        private void LocalisationUI()
        {
            var uiLocalisation = _localisationDataLoad.GetUILocalisation();
            _localizerUI.Localize(uiLocalisation);
        }

        private void LoadData()
        {
            var data = _progress.GetProgress();

            var imageCaptureForSaveUI = _uiFactoryInfo.ImageCaptureForSaveUI;
            var backgroundUI = _uiFactoryInfo.ImageCaptureForSaveUI.BackgroundUI;
            var dialogueUI = _uiFactoryInfo.ImageCaptureForSaveUI.DialogueUI;

            imageCaptureForSaveUI.SetActivePanel(true);

            foreach (var dialoguesData in data.dialogues)
            {
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
    }
}