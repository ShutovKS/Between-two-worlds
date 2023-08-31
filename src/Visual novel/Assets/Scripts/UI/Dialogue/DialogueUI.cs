using Data.Localization.UILocalisation;
using UnityEngine;

namespace UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogueScreenGameObject;

        [field: SerializeField] public AnswerOptionsUI Answers { get; private set; }
        [field: SerializeField] public PersonAvatarUI Person { get; private set; }
        [field: SerializeField] public DialogueTextUI DialogueText { get; private set; }
        [field: SerializeField] public ButtonsUI Buttons { get; private set; }

        public void SetActivePanel(bool value) => _dialogueScreenGameObject.SetActive(value);

        public void SetLocalisation(UILocalisation localisation)
        {
            // Buttons.SetHistoryButtonText(localisation.HistoryButton);
            // Buttons.SetSkipButtonText(localisation.SkipButton);
            // Buttons.SetAutoButtonText(localisation.AutoButton);
            // Buttons.SetFurtherButtonText(localisation.FurtherButton);
        }
    }
}