using System.Collections.Generic;
using Data.Localization.Dialogues;
using Data.Localization.LocalizationMain;
using Data.Localization.UILocalisation;

namespace Infrastructure.Services.LocalisationDataLoad
{
    public interface ILocalisationDataLoadService
    {
        string CurrentLanguage { get; }
        
        void Load(string language);

        IPhrase GetPart(string id);
        UILocalisation GetUILocalisation();
        List<LocalizationMain> GetLocalizationsInfo();
    }
}