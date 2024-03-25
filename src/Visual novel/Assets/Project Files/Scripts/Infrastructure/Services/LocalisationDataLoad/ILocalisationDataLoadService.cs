#region

using System.Collections.Generic;
using Data.Static.Dialogues;
using Data.Static.LocalizationMain;
using Data.Static.UILocalisation;

#endregion

namespace Infrastructure.Services.LocalisationDataLoad
{
    public interface ILocalisationDataLoadService
    {
        string CurrentLanguage { get; }
        void Load(string language);

        string GetUpLastWord(string id);
        IPhrase GetPhraseId(string id);
        UILocalisation GetUILocalisation();
        List<LocalizationMain> GetLocalizationsInfo();
    }
}