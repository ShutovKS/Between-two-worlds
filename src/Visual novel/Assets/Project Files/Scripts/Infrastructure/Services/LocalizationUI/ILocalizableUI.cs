#region

using Data.Static.UILocalisation;

#endregion

namespace Infrastructure.Services.LocalizationUI
{
    public interface ILocalizableUI
    {
        void Localize(UILocalisation localisation);
    }
}