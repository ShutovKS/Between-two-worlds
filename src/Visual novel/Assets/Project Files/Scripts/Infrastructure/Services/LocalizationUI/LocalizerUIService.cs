#region

using System.Collections.Generic;
using Data.Localization.UILocalisation;

#endregion

namespace Infrastructure.Services.LocalizationUI
{
    public class LocalizerUIService : ILocalizerUIService
    {
        private readonly List<ILocalizableUI> _localizables = new();

        public void Localize(UILocalisation localisation)
        {
            foreach (var localizable in _localizables)
            {
                localizable.Localize(localisation);
            }
        }

        public void Register(ILocalizableUI localizableUI)
        {
            _localizables.Add(localizableUI);
        }

        public void Unregister(ILocalizableUI localizableUI)
        {
            _localizables.Remove(localizableUI);
        }

        public void Clear()
        {
            _localizables.Clear();
        }
    }
}