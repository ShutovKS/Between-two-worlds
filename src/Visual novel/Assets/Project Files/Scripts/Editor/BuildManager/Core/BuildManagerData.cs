using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.BuildManager.Core
{
    [Serializable]
    public class BuildManagerData : ScriptableObject, ICloneable
    {
        public bool IsNeedZip;
        public bool IsPassbyBuild;
        public bool IsReleaseBuild;

        public List<BuildData> Builds = new();
        
        public string ScriptingDefineSymbolsDefault;
        
        public string OutputRoot = "Builds/";
        public string MiddlePath = "$NAME_$VERSION_$PLATFORM/$NAME_$VERSION/$NAME$EXECUTABLE";
        public string DirPathForPostProcess = "$NAME_$VERSION_$PLATFORM";
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}