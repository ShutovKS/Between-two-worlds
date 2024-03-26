#region

using System.Collections.Generic;
using Data.Static.Dialogues;
using Data.Static.LocalizationMain;
using Data.Static.UILocalisation;

#endregion

namespace Infrastructure.Services.Localisation
{
    public interface ILocalisationService
    {
        string CurrentLanguage { get; }
        void Load(string language);

        string GetUpLastWord(string id);
        IPhrase GetPhraseId(string id);
        UILocalisation GetUILocalisation();
        List<LocalizationMain> GetLocalizationsInfo();
    }
}