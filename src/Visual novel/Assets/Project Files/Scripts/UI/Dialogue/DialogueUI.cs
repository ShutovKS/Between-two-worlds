#region

using Data.Localization.UILocalisation;
using Features.UI.Scripts.Base;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.Dialogue
{
    public class DialogueUI : BaseScreen, ILocalizableUI
    {
        [SerializeField] private Canvas canvas;

        [field: SerializeField] public AnswerOptionsUI Answers { get; private set; }
        [field: SerializeField] public PersonAvatarUI Person { get; private set; }
        [field: SerializeField] public DialogueTextUI DialogueText { get; private set; }
        [field: SerializeField] public ButtonsUI Buttons { get; private set; }
        [field: SerializeField] public HistoryUI History { get; private set; }

        public void Localize(UILocalisation localisation)
        {
            Buttons.SetHistoryButtonText(localisation.HistoryButton);
            Buttons.SetSkipButtonText(localisation.SkipButton);
            Buttons.SetAutoButtonText(localisation.AutoButton);
            Buttons.SetFurtherButtonText(localisation.FurtherButton);
        }

        public void SetActivePanel(bool value)
        {
            canvas.enabled = value;
        }
    }
}