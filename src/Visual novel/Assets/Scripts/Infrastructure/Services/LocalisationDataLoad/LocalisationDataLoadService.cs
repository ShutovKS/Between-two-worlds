using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Localization.Dialogue;
using Data.Localization.LocalizationMain;
using Data.Localization.UILocalisation;
using UnityEngine;

namespace Infrastructure.Services.LocalisationDataLoad
{
    public class LocalisationDataLoadService : ILocalisationDataLoadService
    {
        public LocalisationDataLoadService()
        {
            var path = $"{Application.streamingAssetsPath}\\Localizations";

            var directoriesNames = Directory.GetDirectories(path);

            foreach (var directoryName in directoriesNames)
            {
                if (!File.Exists($"{directoryName}\\Main.txt")) continue;

                var fileString = File.ReadAllText($"{directoryName}\\Main.txt");
                var strings = fileString.Split("\n");

                var localizationMain = new LocalizationMain
                {
                    PathToDirectory = directoryName,
                    Language = strings[0].Remove(0, strings[0].IndexOf('-'))
                };

                _localizations.Add(localizationMain.Language, localizationMain);
            }
        }

        public string CurrentLanguage { get; private set; }

        private const string PATH_TO_DIALOGUE = "";
        private const string PATH_TO_UI = "UILocalisation";

        private readonly Dictionary<string, LocalizationMain> _localizations = new();
        private readonly Dictionary<string, Part> _partsLocalisation = new();
        private UILocalisation _uiLocalisation = new();

        public void Load(string language)
        {
            CurrentLanguage = language;
            // LoadDialogues(language);
            LoadUILocalisation(language);
        }

        [Obsolete]
        public Part GetPart(string id)
        {
            throw new Exception($"No get part");
        }

        public UILocalisation GetUILocalisation()
        {
            return _uiLocalisation;
        }

        public List<LocalizationMain> GetLocalizationsInfo()
        {
            return _localizations.Values.ToList();
        }

        [Obsolete]
        private void LoadDialogues(string language)
        {
            var json = File.ReadAllText($"{_localizations[language].PathToDirectory}\\{PATH_TO_DIALOGUE}");
            var partsDictionary = new Dictionary<string, Part>();

            var part = JsonUtility.FromJson<Part>(json);
            // Fill in the dialog locale
        }

        private void LoadUILocalisation(string language)
        {
            var json = File.ReadAllText($"{_localizations[language].PathToDirectory}\\{PATH_TO_UI}.json");
            var uiLocalisation = JsonUtility.FromJson<UILocalisation>(json);
            _uiLocalisation = uiLocalisation;
        }
    }
}