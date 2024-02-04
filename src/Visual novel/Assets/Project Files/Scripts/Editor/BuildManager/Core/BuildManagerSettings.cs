#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Editor.BuildManager.Core
{
    public class BuildManagerSettings : ScriptableObject
    {
        public List<BuildSequence> sequences = new() { new BuildSequence() };

        public string scriptingDefineSymbols;

        public void CloneInto(BuildManagerSettings settings)
        {
            scriptingDefineSymbols = settings.scriptingDefineSymbols;

            sequences = new List<BuildSequence>(settings.sequences.Count);
            for (var i = 0; i < settings.sequences.Count; ++i)
            {
                sequences.Add(settings.sequences[i].Clone() as BuildSequence);
            }
        }
    }
}