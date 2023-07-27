using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Services.JsonLoad
{
    public class StaticDataLoad : IStaticDataLoad
    {
        private const string PATH_TO_DIALOGUE = "";
        private readonly Dictionary<string, Part> _partsDictionary;

        public void Load()
        {
            var jsons = Resources.LoadAll<TextAsset>(PATH_TO_DIALOGUE);

            foreach (var json in jsons)
            {
                var jsonString = json.ToString();
                var part = JsonUtility.FromJson<Part>(jsonString);
                _partsDictionary.Add(part.id, part);
            }
        }

        public Part GetPart(string id)
        {
            return _partsDictionary.TryGetValue(id, out var part)
                ? part
                : throw new Exception($"No JSON file by id: {id}");
        }
    }
}