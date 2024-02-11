#region

using System.Collections.Generic;
using Data.Localization.Dialogues;
using Data.Localization.LocalizationMain;
using Data.Localization.UILocalisation;

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