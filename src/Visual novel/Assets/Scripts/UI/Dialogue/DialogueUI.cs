using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

namespace UI.Dialogue
{
    public class DialogueUI : MonoBehaviour, ILocalizableUI
    {
        [SerializeField] private GameObject _dialogueScreenGameObject;

        [field: SerializeField] public AnswerOptionsUI Answers { get; private set; }
        [field: SerializeField] public PersonAvatarUI Person { get; private set; }
        [field: SerializeField] public DialogueTextUI DialogueText { get; private set; }
        [field: SerializeField] public ButtonsUI Buttons { get; private set; }

        public void SetActivePanel(bool value) => _dialogueScreenGameObject.SetActive(value);

        public void Localize(UILocalisation localisation)
        {
            // Buttons.SetHistoryButtonText(localisation.HistoryButton);
            // Buttons.SetSkipButtonText(localisation.SkipButton);
            // Buttons.SetAutoButtonText(localisation.AutoButton);
            // Buttons.SetFurtherButtonText(localisation.FurtherButton);
        }
    }
}