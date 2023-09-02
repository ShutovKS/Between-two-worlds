using Data.Localization.UILocalisation;

namespace Infrastructure.Services.LocalizationUI
{
    public interface ILocalizerUI
    {
        void Localize(UILocalisation localisation);

        void Register(ILocalizableUI localizableUI);
        void Unregister(ILocalizableUI localizableUI);
        void Clear();
    }
}