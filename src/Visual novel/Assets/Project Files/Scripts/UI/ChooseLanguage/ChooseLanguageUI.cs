#region

using UnityEngine;

#endregion

namespace UI.ChooseLanguage
{
    public class ChooseLanguageUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        [field: SerializeField] public ScrollViewLanguagesUI ScrollViewLanguages { get; private set; }

        public void SetActivePanel(bool value)
        {
            canvas.enabled = value;
        }
    }
}