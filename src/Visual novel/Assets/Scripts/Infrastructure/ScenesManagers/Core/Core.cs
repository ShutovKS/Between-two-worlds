using System.Threading.Tasks;
using Data.Dynamic;
using Data.Localization.Dialogues;
using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;

namespace Infrastructure.ScenesManagers.Core
{
    public class Core : MonoBehaviour
    {
        private ILocalisationDataLoadService _localisationDataLoad;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;
        private IAssetsAddressablesProviderService _assetsAddressablesProvider;

        private DynamicData _data;

        private void Awake()
        {
            InitializedServices();
            LoadData();
            // SetDialog(_data.dialogues.idLastDialogue);
            SetDialog("1");
            _uiFactoryInfo.DialogueUI.SetActivePanel(true);
        }

        private void InitializedServices()
        {
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _assetsAddressablesProvider = ServicesContainer.GetService<IAssetsAddressablesProviderService>();
        }

        private void LoadData()
        {
            _data = _saveLoadData.Load();
        }

        private async void SetDialog(string id)
        {
            var dialogue = _localisationDataLoad.GetPart(id);

            if (dialogue is Phrase phrase)
            {
                _uiFactoryInfo.DialogueUI.Answers.SetActiveAnswerOptions(false);

                _uiFactoryInfo.DialogueUI.DialogueText.SetText(phrase.Text);
                _uiFactoryInfo.DialogueUI.DialogueText.SetAuthorName(phrase.Name);

                _uiFactoryInfo.DialogueUI.Person.SetAvatar(
                    await GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));

                _uiFactoryInfo.BackgroundUI.SetBackgroundImage(
                    await GetTexture2D("Backgrounds/" + phrase.BackgroundPath));


                _uiFactoryInfo.DialogueUI.DialogueText.SetBackButtonText("Next");
                _uiFactoryInfo.DialogueUI.DialogueText.RegisterFurtherButtonCallback(
                    () => SetDialog(phrase.IDNextDialog));
            }
            else if (dialogue is Responses response)
            {
                _uiFactoryInfo.DialogueUI.Answers.SetActiveAnswerOptions(true);

                _uiFactoryInfo.DialogueUI.Answers.SetAnswerOptions(
                    (response.ResponseList[0].AnswerText, () => SetDialog(response.ResponseList[0].IDNextDialog)),
                    (response.ResponseList[1].AnswerText, () => SetDialog(response.ResponseList[1].IDNextDialog)),
                    (response.ResponseList[2].AnswerText, () => SetDialog(response.ResponseList[2].IDNextDialog)));
            }
        }

        private async Task<Texture2D> GetTexture2D(string path)
        {
            var texture2D = await _assetsAddressablesProvider.GetAsset<Texture2D>(path);

            return texture2D;
        }
    }
}