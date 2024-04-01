#region

using UnityEngine;

#endregion

namespace UI.ChooseLanguage
{
    public class ChooseLanguageUI : BaseScreen
    {
        [SerializeField] private Canvas canvas;

        [field: SerializeField] public ScrollViewLanguagesUI ScrollViewLanguages { get; private set; }

        public void SetActivePanel(bool value)
        {
            canvas.enabled = value;
        }
    }
}