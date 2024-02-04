#region

using System;
using UnityEditor;

#endregion

namespace Editor.BuildManager.Core
{
    [Serializable]
    public class BuildData : ICloneable
    {
        public bool isEnabled;

        public string outputRoot;
        public string middlePath;
        public string dirPathForPostProcess;

        public bool isPassbyBuild; //Use it to simulate build and give to after build hooks previously build game

        public string scriptingDefineSymbolsOverride;

        public BuildTargetGroup targetGroup;
        public BuildTarget target;
        public BuildOptions options;

        public bool isReleaseBuild; // Maximum compressed build with Release IL2CPP

        public bool needZip;

        public BuildData() : this(
            BuildTargetGroup.Unknown,
            BuildTarget.NoTarget
        )
        {
        }

        public BuildData(BuildTargetGroup targetGroup, BuildTarget target)
        {
            this.targetGroup = targetGroup;
            this.target = target;

            isEnabled = true;
            isPassbyBuild = false;

            scriptingDefineSymbolsOverride = "";

            options = BuildOptions.None;

            outputRoot = "Builds/";
            middlePath = "$NAME_$VERSION_$PLATFORM/$NAME_$VERSION/$NAME$EXECUTABLE";
            dirPathForPostProcess = "$NAME_$VERSION_$PLATFORM";

            isReleaseBuild = false;

            needZip = false;
            dirPathForPostProcess = "$NAME_$VERSION_$PLATFORM";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}