using System.Collections.Generic;
using Editor.BuildManager.Core;
using UnityEditor;

namespace Editor.Window
{
    public static class PredefinedBuildConfigs
    {
        public static BuildSequence testingSequence;
        public static BuildSequence testingSequenceZip;

        public static BuildSequence releaseLocalSequence;
        public static BuildSequence releaseLocalZipSequence;
        public static BuildSequence releaseFullSequence;

        public static BuildSequence passbySequence;

        public static BuildData[] standaloneData = new BuildData[]
        {
            new BuildData(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows),
            new BuildData(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64),
            new BuildData(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64),
            new BuildData(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX),
        };

        public static BuildData[] webData = new BuildData[]
        {
            new BuildData(BuildTargetGroup.WebGL, BuildTarget.WebGL) { middlePath = "$NAME_$VERSION_$PLATFORM/", },
        };

        public static BuildData[] androidData = new BuildData[]
        {
            new BuildData(BuildTargetGroup.Android, BuildTarget.Android)
            {
                middlePath = "$NAME_$VERSION_$PLATFORM$EXECUTABLE",
                dirPathForPostProcess = "$NAME_$VERSION_$PLATFORM$EXECUTABLE",
            },
        };

        public static void Init()
        {
            var dataOriginal = new List<BuildData>();
            var data = new List<BuildData>();

            foreach (var buildData in standaloneData)
            {
                dataOriginal.Add(buildData.Clone() as BuildData);
            }

            foreach (var buildData in webData)
            {
                dataOriginal.Add(buildData.Clone() as BuildData);
            }

            foreach (var buildData in androidData)
            {
                dataOriginal.Add(buildData.Clone() as BuildData);
            }

            FillTestingSequence(ref dataOriginal, ref data);
            FillReleaseSequence(ref dataOriginal, ref data);
        }

        static void FillTestingSequence(ref List<BuildData> dataOriginal, ref List<BuildData> data)
        {
            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].middlePath = data[i].middlePath.Replace("_$VERSION", "");
                data[i].dirPathForPostProcess = data[i].dirPathForPostProcess.Replace("_$VERSION", "");
            }

            testingSequence = new BuildSequence("Testing", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].needZip = true;
                data[i].middlePath = data[i].middlePath.Replace("_$VERSION", "");
                data[i].dirPathForPostProcess = data[i].dirPathForPostProcess.Replace("_$VERSION", "");
            }

            testingSequenceZip = new BuildSequence("Testing + zip", data.ToArray());
            data.Clear();
        }

        static void FillReleaseSequence(ref List<BuildData> dataOriginal, ref List<BuildData> data)
        {
            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                dataOriginal[i].outputRoot += "Releases/";
            }

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
            }

            releaseLocalSequence = new BuildSequence("Release", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].needZip = true;
            }

            releaseLocalZipSequence = new BuildSequence("Release + zip", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].needZip = true;
            }

            releaseFullSequence = new BuildSequence("Release full", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].isPassbyBuild = true;
                data[i].needZip = true;
            }

            passbySequence = new BuildSequence("Passby local release", data.ToArray());
            data.Clear();
        }
    }
}