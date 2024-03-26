﻿#region

using System;

#endregion

namespace Data.Static.Dialogues
{
    [Serializable]
    public class Phrase : IPhrase
    {
        public string IDNextDialog { get; set; }
        public string CharacterAvatarPath { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string BackgroundPath { get; set; }
        public string SoundEffect { get; set; }
        public string ActionTrigger { get; set; }
        public string ID { get; set; }
    }
}