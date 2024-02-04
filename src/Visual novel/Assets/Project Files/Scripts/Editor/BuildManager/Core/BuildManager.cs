#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Editor.Changelog;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

#endregion

namespace Editor.BuildManager.Core
{
    public static class BuildManager
    {
        private static DateTime usedDate;
        private static string buildNameString;
        private static string[] buildsPath;
        private static ChangelogData usedChangelog;

        public static void RunBuildSequnce(BuildManagerSettings settings, BuildSequence sequence,
            ChangelogData changelog)
        {
            // Start init
            usedChangelog = changelog;
            var usedChangelogEntry = changelog.GetLastVersion();
            buildNameString = usedChangelogEntry.GetVersionHeader();

#if GAME_TEMPLATE
            TemplateGameManager.Instance.buildNameString = buildNameString;
            TemplateGameManager.Instance.productName = PlayerSettings.productName;
            EditorUtility.SetDirty(TemplateGameManager.Instance);
#endif
            usedDate = DateTime.Now;
            //End init

            Debug.Log("Start building all");
            var startTime = DateTime.Now;

            Build(settings, sequence);

            Compress(sequence);

            Debug.Log($"End building all. Elapsed time: {DateTime.Now - startTime:mm\\\\:ss}");

#if UNITY_EDITOR_WIN
            ShowExplorer(sequence.builds[^1].outputRoot);
#endif
        }

        private static void Build(BuildManagerSettings settings, BuildSequence sequence)
        {
            var targetBeforeStart = EditorUserBuildSettings.activeBuildTarget;
            var targetGroupBeforeStart = BuildPipeline.GetBuildTargetGroup(targetBeforeStart);
            var definesBeforeStart = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroupBeforeStart);

            buildsPath = new string[sequence.builds.Count];
            for (byte i = 0; i < sequence.builds.Count; ++i)
            {
                var data = sequence.builds[i];

                if (!data.isEnabled)
                {
                    continue;
                }

                buildsPath[i] = BaseBuild(
                    data.targetGroup,
                    data.target,
                    data.options,
                    data.outputRoot + GetPathWithVars(data, data.middlePath),
                    string.Concat(settings.scriptingDefineSymbols, ";", sequence.scriptingDefineSymbolsOverride, ";",
                        data.scriptingDefineSymbolsOverride),
                    data.isPassbyBuild,
                    data.isReleaseBuild
                );
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroupBeforeStart, targetBeforeStart);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroupBeforeStart, definesBeforeStart);
        }

        private static void Compress(BuildSequence sequence)
        {
            for (byte i = 0; i < sequence.builds.Count; ++i)
            {
                if (!sequence.builds[i].needZip || !sequence.builds[i].isEnabled)
                {
                    continue;
                }

                if (sequence.builds[i].target == BuildTarget.Android)
                {
                    Debug.Log("Skip android build to .zip, because .apk files already compressed");
                    continue;
                }

                if (string.IsNullOrEmpty(buildsPath[i]))
                {
                    Debug.LogWarning("[Compressing] Can't find build for " +
                                     $"{GetBuildTargetExecutable(sequence.builds[i].target)}");
                }
                else
                {
                    BaseCompress(sequence.builds[i].outputRoot + GetPathWithVars(sequence.builds[i],
                        sequence.builds[i].dirPathForPostProcess));
                }
            }
        }

        #region Convert to strings

        public static string GetPathWithVars(BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{usedDate.Date.Year % 100}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{usedDate.Date.Year}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{usedDate.Hour}_{usedDate.Minute}");
            s = s.Replace("$EXECUTABLE", GetBuildTargetExecutable(data.target));
            return s;
        }

        public static string GetPathWithVarsForZip(BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{usedDate.Date.Year % 100}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{usedDate.Date.Year}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{usedDate.Hour}_{usedDate.Minute}");

            s = s.Contains("$EXECUTABLE")
                ? s.Replace("$EXECUTABLE", GetBuildTargetExecutable(data.target))
                : s + ".zip";

            return s;
        }

        public static string ConvertBuildTargetToString(BuildTarget target)
        {
            return target switch
            {
                BuildTarget.StandaloneOSX => "OSX",
                BuildTarget.StandaloneWindows => "Windows32",
                BuildTarget.StandaloneWindows64 => "Windows64",
                BuildTarget.StandaloneLinux64 => "Linux",
                _ => target.ToString()
            };
        }

        public static string GetProductName()
        {
            return PlayerSettings.productName
                    .Replace(' ', '_')
                    .Replace('/', '_')
                    .Replace('\\', '_')
                    .Replace(':', '_')
                    .Replace('*', '_')
                    .Replace('?', '_')
                    .Replace('"', '_')
                    .Replace('<', '_')
                    .Replace('>', '_')
                    .Replace('|', '_')
                ;
        }

        public static string GetBuildTargetExecutable(BuildTarget target)
        {
            return target switch
            {
                BuildTarget.StandaloneWindows => ".exe",
                BuildTarget.StandaloneWindows64 => ".exe",
                BuildTarget.StandaloneLinux64 => "x86_64",
                BuildTarget.StandaloneOSX => "",
                BuildTarget.iOS => ".ipa",
                BuildTarget.Android => ".apk",
                BuildTarget.WebGL => "",
                _ => ""
            };
        }

        #endregion

        #region Base methods

        private static string BaseBuild(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget,
            BuildOptions buildOptions, string buildPath, string definesSymbols, bool isPassbyBuild, bool isReleaseBuild)
        {
            if (isPassbyBuild)
            {
                return buildPath;
            }

            if (buildTarget == BuildTarget.Android
                && PlayerSettings.Android.useCustomKeystore
                && string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = "keystore";
            }

            if (isReleaseBuild)
            {
                switch (buildTargetGroup)
                {
                    case BuildTargetGroup.Standalone:
                    {
                        buildOptions |= BuildOptions.CompressWithLz4;

                        if (buildTarget == BuildTarget.StandaloneWindows ||
                            buildTarget == BuildTarget.StandaloneWindows64 ||
                            buildTarget == BuildTarget.StandaloneLinux64)
                        {
                            PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                        }
                        else
                        {
                            PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                        }

                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Master);
                        break;
                    }
                    case BuildTargetGroup.Android:
                    {
                        buildOptions |= BuildOptions.CompressWithLz4;

                        PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Master);

                        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
                        break;
                    }
                    case BuildTargetGroup.WebGL:
                    {
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Master);
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"{buildTargetGroup} is unsupported for release builds. " +
                                         "No optimizations applied");
                        break;
                    }
                }
            }
            else
            {
                switch (buildTargetGroup)
                {
                    case BuildTargetGroup.Standalone:
                    {
                        buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);

                        PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Debug);
                        break;
                    }
                    case BuildTargetGroup.Android:
                    {
                        buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);

                        PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.Mono2x);
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Debug);

                        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                        break;
                    }
                    case BuildTargetGroup.WebGL:
                    {
                        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup,
                            Il2CppCompilerConfiguration.Debug);
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"{buildTargetGroup} is unsupported for debug builds. " +
                                         "No optimizations applied");
                        break;
                    }
                }
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = buildPath,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = buildOptions
            };

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, definesSymbols);
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"{summary.platform} succeeded.  \t " +
                          $"Time: {summary.totalTime:mm\\\\:ss}  \t " +
                          $"Size: {summary.totalSize / 1048576} Mb");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.Log($"{summary.platform} failed.   \t " +
                          $"Time: {summary.totalTime:mm\\\\:ss}  \t " +
                          $"Size: {summary.totalSize / 1048576} Mb" +
                          "\n" +
                          $"Warnings: {summary.totalWarnings}" + "\n" +
                          $"Errors:   {summary.totalErrors}"
                );
            }

            return summary.result == BuildResult.Succeeded ? buildPath : "";
        }

        private static void BaseCompress(string dirPath)
        {
            using var zip = new ZipFile();
            var startTime = DateTime.Now;
            if (Directory.Exists(dirPath))
            {
                zip.AddDirectory(dirPath);
            }
            else
            {
                zip.AddFile(dirPath);
            }

            zip.Save(dirPath + ".zip");

            long uncompresedSize = 0;
            long compresedSize = 0;
            foreach (var e in zip.Entries)
            {
                uncompresedSize += e.UncompressedSize;
                compresedSize += e.CompressedSize;
            }

            Debug.Log($"Make {dirPath}.zip.  \t " +
                      $"Time: {$"{DateTime.Now - startTime:mm\\:ss}"}  \t " +
                      $"Size: {uncompresedSize / 1048576} Mb - {compresedSize / 1048576} Mb");
        }

        #endregion

        #region Helpers

        private static void ShowExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\"); // explorer doesn't like front slashes

            var findFile = false;
            var di = new DirectoryInfo(itemPath);
            foreach (var fi in di.GetFiles())
            {
                if (fi.Name != "." && fi.Name != ".." && fi.Name != "Thumbs.db")
                {
                    itemPath = fi.FullName;
                    findFile = true;
                    break;
                }
            }

            if (!findFile)
            {
                foreach (var fi in di.GetDirectories())
                {
                    if (fi.Name != "." && fi.Name != ".." && fi.Name != "Thumbs.db")
                    {
                        itemPath = fi.FullName;
                        break;
                    }
                }
            }

            Process.Start("explorer.exe", "/select," + itemPath);
        }

        private static void CreateAllFodersBeforePath(string path)
        {
            var dirs = ("Assets/" + path).Split('/');
            var allPath = dirs[0];
            for (var i = 1; i < dirs.Length - 1; ++i)
            {
                if (!AssetDatabase.IsValidFolder(allPath + "/" + dirs[i]))
                {
                    AssetDatabase.CreateFolder(allPath, dirs[i]);
                }

                allPath = allPath + "/" + dirs[i];
            }
        }

        #endregion
    }
}