using Data.Localization.UILocalisation;
using UnityEngine;

namespace UI.Confirmation
{
    public class ConfirmationUI : MonoBehaviour
    {
        [SerializeField] private GameObject _confirmationScreenGameObject;

        [field: SerializeField] public ConfirmationButtonsUI Buttons { get; private set; }
        [field: SerializeField] public ConfirmationTextUI Text { get; private set; }

        public void SetActivePanel(bool value) => _confirmationScreenGameObject.SetActive(value);

        public void SetLocalisation(UILocalisation localisation)
        {
            // Text.SetTitleText(localisation.Title);
            // Buttons.SetYesButtonText(localisation.YesButton);
            // Buttons.SetNoButtonText(localisation.NoButton);
        }
    }
}