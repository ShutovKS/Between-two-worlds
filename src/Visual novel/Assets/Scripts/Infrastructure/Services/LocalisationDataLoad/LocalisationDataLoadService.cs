using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Data.Localization.Dialogues;
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

        private string _path;
        private const string PATH_TO_DIALOGUE = "Dialogues.xml";
        private const string PATH_TO_UI = "UILocalisation.json";

        private readonly Dictionary<string, LocalizationMain> _localizations = new();
        private readonly Dictionary<string, IPhrase> _dialogues = new();
        private UILocalisation _uiLocalisation = new();

        public void Load(string language)
        {
            CurrentLanguage = language;
            _path = _localizations[language].PathToDirectory;
            LoadUILocalisation();
            LoadDialogues();
        }

        public IPhrase GetPart(string id)
        {
            return _dialogues.TryGetValue(id, out var phrase)
                ? phrase
                : throw new Exception($"No get part on id: {id}");
        }

        public UILocalisation GetUILocalisation()
        {
            return _uiLocalisation;
        }

        public List<LocalizationMain> GetLocalizationsInfo()
        {
            return _localizations.Values.ToList();
        }

        private void LoadDialogues()
        {
            var serializer = new XmlSerializer(typeof(object[]), new[] { typeof(Phrase), typeof(Responses) });
            object[] deserializedData;
            using (TextReader reader = new StreamReader($"{_path}\\{PATH_TO_DIALOGUE}"))
            {
                deserializedData = (object[])serializer.Deserialize(reader);
            }

            foreach (var item in deserializedData)
            {
                switch (item)
                {
                    case Phrase phrase:
                        _dialogues.Add(phrase.ID, phrase);
                        break;
                    case Responses response:
                        _dialogues.Add(response.ID, response);
                        break;
                }
            }
        }

        private void LoadUILocalisation()
        {
            var json = File.ReadAllText($"{_path}\\{PATH_TO_UI}");
            var uiLocalisation = JsonUtility.FromJson<UILocalisation>(json);
            _uiLocalisation = uiLocalisation;
        }
    }
}