using Data.Localization.UILocalisation;

namespace Infrastructure.Services.LocalizationUI
{
    public interface ILocalizableUI
    {
        void Localize(UILocalisation localisation);
    }
}