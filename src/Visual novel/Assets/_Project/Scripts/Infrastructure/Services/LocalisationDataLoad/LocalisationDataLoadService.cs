#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Data.Localization.Dialogues;
using Data.Localization.LocalizationMain;
using Data.Localization.UILocalisation;
using UnityEngine;

#endregion

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
                    Language = strings[0].Remove(0, strings[0].IndexOf('-') + 1)
                };

                _localizations.Add(localizationMain.Language, localizationMain);
            }
        }

        private const string PATH_TO_DIALOGUE = "Dialogues.xml";
        private const string PATH_TO_UI = "UILocalisation.json";
        private const string PATH_TO_LAST_WORDS = "LastWords.txt";
        private readonly Dictionary<string, IPhrase> _dialogues = new();
        private readonly Dictionary<string, LocalizationMain> _localizations = new();
        private readonly Dictionary<string, string> _lastWords = new();
        private UILocalisation _uiLocalisation = new();
        private string _path;

        public string CurrentLanguage { get; private set; }

        public void Load(string language)
        {
            CurrentLanguage = language;
            _path = _localizations[language].PathToDirectory;
            LoadUILocalisation();
            LoadDialogues();
            LoadLastWords();
        }
        public string GetUpLastWord(string id)
        {
            _lastWords.TryGetValue(id, out var lastWord);
            if (lastWord == null) Debug.LogError($"No get last word on id: {id}");
            return lastWord;
        }

        public IPhrase GetPhraseId(string id)
        {
            _dialogues.TryGetValue(id, out var phrase);
            if (phrase == null) Debug.LogError($"No get phrase on id: {id}");
            return phrase;
        }

        public UILocalisation GetUILocalisation()
        {
            return _uiLocalisation;
        }

        public List<LocalizationMain> GetLocalizationsInfo()
        {
            return _localizations.Values.ToList();
        }

        #region Load

        private void LoadLastWords()
        {
            using TextReader reader = new StreamReader($"{_path}\\{PATH_TO_LAST_WORDS}");
            var fileString = reader.ReadToEnd();
            var strings = fileString.Split("\n");
            _lastWords.Clear();
            foreach (var str in strings)
            {
                var array = str.Split('-');
                _lastWords.Add(array[0], array[1]);
            }
        }

        private void LoadDialogues()
        {
            var serializer = new XmlSerializer(typeof(object[]), new[] { typeof(Phrase), typeof(Responses) });
            object[] deserializedData;
            using (TextReader reader = new StreamReader($"{_path}\\{PATH_TO_DIALOGUE}"))
            {
                deserializedData = (object[])serializer.Deserialize(reader);
            }
            _dialogues.Clear();
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

        #endregion

    }
}
