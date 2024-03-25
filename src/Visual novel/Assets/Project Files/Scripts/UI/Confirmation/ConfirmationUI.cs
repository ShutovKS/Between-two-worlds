#region

using Data.Localization.UILocalisation;
using Features.UI.Scripts.Base;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.Confirmation
{
    public class ConfirmationUI : BaseScreen, ILocalizableUI
    {
        [SerializeField] private Canvas canvas;

        [field: SerializeField] public ConfirmationButtonsUI Buttons { get; private set; }
        [field: SerializeField] public ConfirmationTextUI Text { get; private set; }

        public void Localize(UILocalisation localisation)
        {
            Text.SetTitleText(localisation.Title);
            Buttons.SetYesButtonText(localisation.YesButton);
            Buttons.SetNoButtonText(localisation.NoButton);
        }

        public void SetActivePanel(bool value)
        {
            canvas.enabled = value;
        }
    }
}