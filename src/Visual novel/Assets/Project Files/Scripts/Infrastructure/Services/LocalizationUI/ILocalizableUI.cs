#region

using Data.Localization.UILocalisation;

#endregion

namespace Infrastructure.Services.LocalizationUI
{
    public interface ILocalizableUI
    {
        void Localize(UILocalisation localisation);
    }
}