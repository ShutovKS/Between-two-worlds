#region

using System;
using Editor.ScriptingDefineSymbols;
using UnityEditor;

#endregion

namespace Editor.BuildManager.Core
{
    [Serializable]
    public class BuildData : ICloneable
    {
        public bool isEnabled = true;

        public bool isPassbyBuild = false; //Use it to simulate build and give to after build hooks previously build game
        public bool isReleaseBuild = false;
        public bool isCompress = false;

        public string scriptingDefineSymbols = "";
        
        public string buildPath = "Builds/BuildDirectory";

        public AddonsUsedType addonsUsed;
        public BuildOptions options;
        public BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
        public BuildTarget target = BuildTarget.NoTarget;


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}