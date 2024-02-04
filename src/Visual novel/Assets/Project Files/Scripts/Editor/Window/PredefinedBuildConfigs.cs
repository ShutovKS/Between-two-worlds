using System.Collections.Generic;
using System.Linq;
using Editor.BuildManager.Core;
using UnityEditor;

namespace Editor.Window
{
    public static class PredefinedBuildConfigs
    {
        public static BuildSequence TestingSequence;
        public static BuildSequence TestingSequenceZip;

        public static BuildSequence ReleaseLocalSequence;
        public static BuildSequence ReleaseLocalZipSequence;
        public static BuildSequence ReleaseFullSequence;

        public static BuildSequence PassbySequence;

        public static readonly BuildData[] StandaloneData =
        {
            new(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows),
            new(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64),
            new(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64),
            new(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX),
        };

        public static readonly BuildData[] WebData =
        {
            new(BuildTargetGroup.WebGL, BuildTarget.WebGL) { middlePath = "$NAME_$VERSION_$PLATFORM/", },
        };

        public static readonly BuildData[] AndroidData =
        {
            new(BuildTargetGroup.Android, BuildTarget.Android)
            {
                middlePath = "$NAME_$VERSION_$PLATFORM$EXECUTABLE",
                dirPathForPostProcess = "$NAME_$VERSION_$PLATFORM$EXECUTABLE",
            },
        };

        public static void Init()
        {
            var data = new List<BuildData>();

            var dataOriginal = StandaloneData.Select(buildData => buildData.Clone() as BuildData).ToList();

            dataOriginal.AddRange(WebData.Select(buildData => buildData.Clone() as BuildData));

            dataOriginal.AddRange(AndroidData.Select(buildData => buildData.Clone() as BuildData));

            FillTestingSequence(ref dataOriginal, ref data);
            FillReleaseSequence(ref dataOriginal, ref data);
        }

        private static void FillTestingSequence(ref List<BuildData> dataOriginal, ref List<BuildData> data)
        {
            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].middlePath = data[i].middlePath.Replace("_$VERSION", "");
                data[i].dirPathForPostProcess = data[i].dirPathForPostProcess.Replace("_$VERSION", "");
            }

            TestingSequence = new BuildSequence("Testing", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].needZip = true;
                data[i].middlePath = data[i].middlePath.Replace("_$VERSION", "");
                data[i].dirPathForPostProcess = data[i].dirPathForPostProcess.Replace("_$VERSION", "");
            }

            TestingSequenceZip = new BuildSequence("Testing + zip", data.ToArray());
            data.Clear();
        }

        private static void FillReleaseSequence(ref List<BuildData> dataOriginal, ref List<BuildData> data)
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

            ReleaseLocalSequence = new BuildSequence("Release", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].needZip = true;
            }

            ReleaseLocalZipSequence = new BuildSequence("Release + zip", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].needZip = true;
            }

            ReleaseFullSequence = new BuildSequence("Release full", data.ToArray());
            data.Clear();

            for (var i = 0; i < dataOriginal.Count; ++i)
            {
                data.Add(dataOriginal[i].Clone() as BuildData);
                data[i].isReleaseBuild = true;
                data[i].isPassbyBuild = true;
                data[i].needZip = true;
            }

            PassbySequence = new BuildSequence("Passby local release", data.ToArray());
            data.Clear();
        }
    }
}