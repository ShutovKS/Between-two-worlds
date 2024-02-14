#region

using System;
using Unit.Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#endregion

namespace UI.ChooseLanguage
{
    [Serializable]
    public class ScrollViewLanguagesUI
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private GameObject languagePrefab;

        public void AddLanguageInScrollView(string languageName, Sprite flagSprite, UnityAction onClick)
        {
            var instance = Object.Instantiate(languagePrefab, contentTransform);

            instance.SetActive(true);

            var languageUI = instance.GetComponent<LanguageUI>();
            languageUI.buttonComponent.RegisterNewCallback(onClick);
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