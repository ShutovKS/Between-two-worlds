#region

using System;
using System.Collections.Generic;

#endregion

namespace Editor.BuildManager.Core
{
    [Serializable]
    public class BuildSequence : ICloneable
    {
        public bool isEnabled;

        public string editorName;

        public string scriptingDefineSymbolsOverride;

        public List<BuildData> builds;

        public BuildSequence() : this("New build sequence", new BuildData())
        {
        }

        public BuildSequence(string editorName, params BuildData[] builds)
        {
            this.editorName = editorName;
            this.builds = new List<BuildData>(builds);

            scriptingDefineSymbolsOverride = "";

            isEnabled = true;
        }

        public object Clone()
        {
            var sequence = MemberwiseClone() as BuildSequence;

            sequence.builds = new List<BuildData>(builds.Count);
            for (var i = 0; i < builds.Count; ++i)
            {
                sequence.builds.Add(builds[i].Clone() as BuildData);
            }

            return sequence;
        }
    }
}