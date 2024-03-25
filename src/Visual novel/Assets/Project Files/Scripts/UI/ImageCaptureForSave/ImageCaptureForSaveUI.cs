using Data.Localization.UILocalisation;
using Features.UI.Scripts.Base;
using Infrastructure.Services.LocalizationUI;
using UI.Background;
using UI.Dialogue;
using UnityEngine;

namespace UI.ImageCaptureForSave
{
    public class ImageCaptureForSaveUI : BaseScreen, ILocalizableUI
    {
        [field: SerializeField] public BackgroundUI BackgroundUI { get; private set; }
        [field: SerializeField] public DialogueUI DialogueUI { get; private set; }
        
        public void Localize(UILocalisation localisation)
        {
            DialogueUI.Localize(localisation);
        }
        
        public void SetActivePanel(bool isActive)
        {
            BackgroundUI.SetActivePanel(isActive);
            DialogueUI.SetActivePanel(isActive);
        }
    }
}