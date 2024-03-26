using System.Threading.Tasks;
using Data.Static.Dialogues;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.WindowsService;
using Tools.Camera;
using UI.ImageCaptureForSave;
using UnityEngine;
using UnityEngine.Events;
using Resources = Tools.Extensions.Resources;

namespace Infrastructure.Services.ScreenshotsOfSaves
{
    public class ScreenshotsOfSavesService : IScreenshotsOfSavesService
    {
        public ScreenshotsOfSavesService(ILocalisationService localisationService, IWindowService windowService)
        {
            _localisationService = localisationService;
            _windowService = windowService;
        }

        private readonly ILocalisationService _localisationService;
        private readonly IWindowService _windowService;

        public async Task<Texture2D> GetImage(string idDialog)
        {
            var phraseId = _localisationService.GetPhraseId(idDialog);

            var imageCaptureForSaveUI = await _windowService.OpenAndGetComponent<ImageCaptureForSaveUI>(
                WindowID.ImageCaptureForSave);

            switch (phraseId)
            {
                case Phrase phrase:
                {
                    SetUpForPhrase(imageCaptureForSaveUI, phrase);
                    break;
                }
                case Responses response:
                {
                    SetUpForResponses(imageCaptureForSaveUI, response);
                    break;
                }
            }

            var texture2D = CameraTextureCapture.CaptureCameraView(
                imageCaptureForSaveUI.BackgroundUI.GetComponent<Canvas>(),
                imageCaptureForSaveUI.DialogueUI.GetComponent<Canvas>());

            _windowService.Close(WindowID.ImageCaptureForSave);
            
            return texture2D;
        }

        private static void SetUpForPhrase(ImageCaptureForSaveUI imageCaptureForSaveUI, Phrase phrase)
        {
            imageCaptureForSaveUI.BackgroundUI.SetImage(Resources.GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
            imageCaptureForSaveUI.DialogueUI.Answers.SetActiveAnswerOptions(false);
            imageCaptureForSaveUI.DialogueUI.DialogueText.SetAuthorName(phrase.Name);
            imageCaptureForSaveUI.DialogueUI.DialogueText.SetText(phrase.Text);
            if (phrase.CharacterAvatarPath == null)
            {
                imageCaptureForSaveUI.DialogueUI.Person.SetActionAvatar(false);
            }
            else
            {
                imageCaptureForSaveUI.DialogueUI.Person.SetActionAvatar(true);
                imageCaptureForSaveUI.DialogueUI.Person.SetAvatar(
                    Resources.GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
            }
        }

        private static void SetUpForResponses(ImageCaptureForSaveUI imageCaptureForSaveUI, Responses response)
        {
            imageCaptureForSaveUI.DialogueUI.Answers.SetActiveAnswerOptions(true);

            var tuples = new (string, UnityAction)[response.ResponseList.Length];
            for (var i = 0; i < response.ResponseList.Length; i++)
            {
                tuples[i] = (response.ResponseList[i].AnswerText, null);
            }

            imageCaptureForSaveUI.DialogueUI.Answers.SetAnswerOptions(tuples);
        }
    }
}