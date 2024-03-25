﻿#region

using Data.Static.UILocalisation;

#endregion

namespace Infrastructure.Services.LocalizationUI
{
    public interface ILocalizerUIService
    {
        void Localize(UILocalisation localisation);

        void Register(ILocalizableUI localizableUI);
        void Unregister(ILocalizableUI localizableUI);
        void Clear();
    }
}