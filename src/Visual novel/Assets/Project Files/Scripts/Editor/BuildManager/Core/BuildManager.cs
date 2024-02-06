#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Editor.ScriptingDefineSymbols;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

#endregion

namespace Editor.BuildManager.Core
{
    public static class BuildManager
    {
        public static BuildManagerData Settings { get; set; }

        private static DateTime _usedDate;
        private static string _shownPath;

        public static void RunBuild()
        {
            if (Settings == null || Settings.Builds.Count == 0)
            {
                Debug.LogError("No data available");
                return;
            }

            var targetBeforeStart = EditorUserBuildSettings.activeBuildTarget;
            var targetGroupBeforeStart = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTargetStart = NamedBuildTarget.FromBuildTargetGroup(targetGroupBeforeStart);
            var scriptingDefineSymbolsStart = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

            _usedDate = DateTime.Now;
            _shownPath = Settings.OutputRoot;

            var startTime = DateTime.Now;

            for (byte i = 0; i < Settings.Builds.Count; ++i)
            {
                var buildData = Settings.Builds[i];

                if (!buildData.isEnabled)
                {
                    continue;
                }

                buildData.buildPath = Settings.OutputRoot + GetPathWithVars(buildData, Settings.MiddlePath);

                BaseBuild(
                    buildData.targetGroup,
                    buildData.target,
                    buildData.options,
                    buildData.buildPath,
                    buildData.addonsUsed,
                    buildData.isPassbyBuild,
                    buildData.isReleaseBuild
                );
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroupBeforeStart, targetBeforeStart);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, scriptingDefineSymbolsStart);

            for (byte i = 0; i < Settings.Builds.Count; ++i)
            {
                var buildData = Settings.Builds[i];

                if (!buildData.isCompress || !buildData.isEnabled || buildData.targetGroup == BuildTargetGroup.Android)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(buildData.buildPath))
                {
                    BaseCompress(Settings.OutputRoot + GetPathWithVars(buildData, Settings.DirPathForPostProcess));
                    return;
                }

                Debug.LogWarning("Can't find build for " + $"{GetBuildTargetExecutable(buildData.target)}");
            }

            Debug.Log($@"End building all. Elapsed time: {(DateTime.Now - startTime).ToString()}");

#if UNITY_EDITOR_WIN
            if (string.IsNullOrEmpty(_shownPath) == false)
            {
                ShowExplorer(_shownPath);
            }
#endif
        }

        #region Loading Data

        public static void LoadSettings()
        {
            const string SETTINGS_DEFAULT_PATH = "Assets/Editor/Setting/BuildSequences.asset";
            const string SETTINGS_PATH_KEY = "BuildManagerWindow.SettingsPath";

            var settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");

            if (!string.IsNullOrEmpty(settingsPath))
            {
                Settings = AssetDatabase.LoadAssetAtPath<BuildManagerData>(settingsPath);
                if (Settings == null)
                {
                    settingsPath = null;
                }
            }

            if (string.IsNullOrEmpty(settingsPath))
            {
                var guids = AssetDatabase.FindAssets("t:BuildManagerSettings", new[] { "Assets" });
                if (guids.Length >= 2)
                {
                    Debug.LogError("2+ BuildManagerSettings exist. Consider on using only 1 setting. " +
                                   "The first one will be used.");
                }

                if (guids.Length != 0)
                {
                    settingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    PlayerPrefs.SetString(SETTINGS_PATH_KEY, settingsPath);
                    Settings = AssetDatabase.LoadAssetAtPath<BuildManagerData>(settingsPath);
                }

                Debug.Log("2");
            }

            if (Settings == null)
            {
                Settings = (BuildManagerData)ScriptableObject.CreateInstance(typeof(BuildManagerData));
                AssetDatabase.CreateAsset(Settings, SETTINGS_DEFAULT_PATH);
                PlayerPrefs.SetString(SETTINGS_PATH_KEY, SETTINGS_DEFAULT_PATH);
            }
        }

        #endregion

        #region Base methods

        private static void BaseBuild(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget,
            BuildOptions buildOptions, string buildPath, AddonsUsedType addonsUsedType, bool isPassbyBuild,
            bool isReleaseBuild)
        {
            if (isPassbyBuild)
            {
                return;
            }

            if (buildTargetGroup == BuildTargetGroup.Android
                && PlayerSettings.Android.useCustomKeystore
                && string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = "keystore";
            }

            var namedBuildTargetStart = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            
            var releaseScriptingImplementation = isReleaseBuild
                ? ScriptingImplementation.IL2CPP
                : ScriptingImplementation.Mono2x;

            switch (buildTargetGroup, isReleaseBuild)
            {
                case (BuildTargetGroup.Standalone, true):
                    buildOptions |= BuildOptions.CompressWithLz4;
                    PlayerSettings.SetScriptingBackend(namedBuildTargetStart, releaseScriptingImplementation);
                    break;
                case (BuildTargetGroup.Standalone, false):
                    buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);
                    PlayerSettings.SetScriptingBackend(namedBuildTargetStart, releaseScriptingImplementation);
                    break;

                case (BuildTargetGroup.Android, true):
                    buildOptions |= BuildOptions.CompressWithLz4;
                    PlayerSettings.SetScriptingBackend(namedBuildTargetStart, releaseScriptingImplementation);
                    break;
                case (BuildTargetGroup.Android, false):
                    buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);
                    PlayerSettings.SetScriptingBackend(namedBuildTargetStart, releaseScriptingImplementation);
                    break;

                case (BuildTargetGroup.WebGL, true):
                    break;
                case (BuildTargetGroup.WebGL, false):
                    break;
            }

            

            var releaseCompilerType = isReleaseBuild
                ? Il2CppCompilerConfiguration.Master
                : Il2CppCompilerConfiguration.Debug;

            PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTargetStart, releaseCompilerType);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = buildPath,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = buildOptions,
            };

            var oldScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

            var addonsUsedScriptingDefineSymbols = AddonsUsed.GetAddonsUsed(addonsUsedType);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, addonsUsedScriptingDefineSymbols);

            BuildSummary summary = default;

            try
            {
                var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                summary = report!.summary;
            }
            catch (Exception)
            {
                //
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, oldScriptingDefineSymbols);

            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"{summary.platform} succeeded.  \t " +
                              $"Time: {summary.totalTime.ToString()}  \t " +
                              $"Size: {summary.totalSize / 1024 / 1024} Mb" + "\n");
                    break;
                case BuildResult.Failed:
                    Debug.Log($"{summary.platform} failed.   \t " +
                              $"Time: {summary.totalTime.ToString()}  \t " +
                              $"Size: {summary.totalSize / 1024 / 1024} Mb" + "\n" +
                              "\n" +
                              $"Warnings: {summary.totalWarnings}" + "\n" +
                              $"Errors:   {summary.totalErrors}"
                    );
                    break;
                case BuildResult.Unknown:
                    Debug.Log($"{summary.platform} unknown.  \t " +
                              $"Time: {summary.totalTime.ToString()}");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log($"{summary.platform} cancelled. \t " +
                              $"Time: {summary.totalTime.ToString()}");
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
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
                      $"Time: {(DateTime.Now - startTime).ToString()}  \t " +
                      $"Size: {uncompresedSize / 1048576} Mb - {compresedSize / 1048576} Mb");
        }

        #endregion

        #region Convert to strings

        public static string GetPathWithVars(BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{_usedDate.Date.Year % 100}_{_usedDate.Date.Month}_{_usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{_usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{_usedDate.Date.Year}_{_usedDate.Date.Month}_{_usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{_usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{_usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{_usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{_usedDate.Hour}_{_usedDate.Minute}");
            s = s.Replace("$ADDONS", $"{AddonsUsed.GetAddonsUsedName(data.addonsUsed)}");
            s = s.Replace("$EXECUTABLE", GetBuildTargetExecutable(data.target));
            return s;
        }

        public static string GetPathWithVarsForZip(BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{_usedDate.Date.Year % 100}_{_usedDate.Date.Month}_{_usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{_usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{_usedDate.Date.Year}_{_usedDate.Date.Month}_{_usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{_usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{_usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{_usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{_usedDate.Hour}_{_usedDate.Minute}");

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

        #region Helpers

        private static void ShowExplorer(string itemPath)
        {
            itemPath = itemPath.Replace(@"/", @"\"); // Explorer не любит косые черты спереди

            var findFile = false;

            var di = new DirectoryInfo(itemPath);

            foreach (var fi in di.GetFiles())
            {
                if (fi.Name is "." or ".." or "Thumbs.db")
                {
                    continue;
                }

                itemPath = fi.FullName;
                findFile = true;
                break;
            }

            if (!findFile)
            {
                foreach (var fi in di.GetDirectories())
                {
                    if (fi.Name is "." or ".." or "Thumbs.db")
                    {
                        continue;
                    }

                    itemPath = fi.FullName;
                    break;
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