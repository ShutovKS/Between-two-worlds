using System;
using UnityEngine;

namespace Data.Dynamic.Language
{
    [Serializable]
    public class LanguageCurrent
    {
        public LanguageCurrent()
        {
            Language = Application.systemLanguage;
        }

        public LanguageCurrent(SystemLanguage language)
        {
            Language = language;
        }

        public SystemLanguage Language;
    }
}