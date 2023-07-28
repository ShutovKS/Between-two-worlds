using System;
using Data.Dynamic.Language;

namespace Data.Dynamic
{
    [Serializable]
    public class DynamicData
    {
        public DynamicData()
        {
            LanguageCurrent = new LanguageCurrent();
        }

        public LanguageCurrent LanguageCurrent;
    }
}