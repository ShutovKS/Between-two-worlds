#region

using System;
using Tools.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace UI.ChooseLanguage
{
    [Serializable]
    public class ScrollViewLanguagesUI
    {
        public event Action<string> OnSelectLanguage;
        
        [SerializeField] private Transform contentTransform;
        [SerializeField] private GameObject languagePrefab;

        public void AddLanguageInScrollView(string languageName, Sprite flagSprite)
        {
            var instance = Object.Instantiate(languagePrefab, contentTransform);

            instance.SetActive(true);

            var languageUI = instance.GetComponent<LanguageUI>();
            languageUI.buttonComponent.RegisterNewCallback(() => OnSelectLanguage?.Invoke(languageName));
            languageUI.textComponent.text = languageName;
            languageUI.imageComponent.sprite = flagSprite;
        }

        public void RemoveAllLanguagesInScrollView()
        {
            foreach (Object variable in contentTransform)
            {
                Object.Destroy(variable);
            }
        }
    }
}