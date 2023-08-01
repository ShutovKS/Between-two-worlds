using System.Collections.Generic;
using Data.Localization.Dialogue;
using Data.Localization.LocalizationMain;
using Data.Localization.UILocalisation;

namespace Infrastructure.Services.LocalisationDataLoad
{
    public interface ILocalisationDataLoadService
    {
        void Load(string language);

        Part GetPart(string id);
        UILocalisation GetUILocalisation();
        List<LocalizationMain> GetLocalizationsInfo();
    }
}