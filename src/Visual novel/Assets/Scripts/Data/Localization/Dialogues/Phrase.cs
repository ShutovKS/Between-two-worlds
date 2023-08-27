using System;
using System.Collections.Generic;

namespace Data.Localization.Dialogues
{
    [Serializable]
    public class Phrase : IPhrase
    {
        public string ID { get; set; }
        public string IDNextDialog { get; set; }
        public string CharacterAvatarPath { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string BackgroundPath { get; set; }
        public string SoundEffect { get; set; }
    }
}