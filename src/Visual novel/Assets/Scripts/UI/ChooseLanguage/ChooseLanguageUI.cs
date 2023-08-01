using UnityEngine;

namespace UI.ChooseLanguage
{
    public class ChooseLanguageUI : MonoBehaviour
    {
        [SerializeField] private GameObject _shooseLanguageScreenGameObject;

        [field: SerializeField] public ScrollViewLanguagesUI ScrollViewLanguages { get; private set; }

        public void SetActivePanel(bool value) => _shooseLanguageScreenGameObject.SetActive(value);
    }
}