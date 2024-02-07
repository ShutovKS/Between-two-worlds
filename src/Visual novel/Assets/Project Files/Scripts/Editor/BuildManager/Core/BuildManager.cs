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
using static Editor.BuildManager.Core.ConvertToStrings;

#endregion

namespace Editor.BuildManager.Core
{
    public static class BuildManager
    {
        public static BuildManagerData Settings { get; private set; }

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
            var definesBeforeStart = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

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

                buildData.buildPath = Settings.OutputRoot + GetPathWithVars(_usedDate, buildData, Settings.MiddlePath);

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
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, definesBeforeStart);

            for (byte i = 0; i < Settings.Builds.Count; ++i)
            {
                var buildData = Settings.Builds[i];

                if (!buildData.isCompress || !buildData.isEnabled || buildData.targetGroup == BuildTargetGroup.Android)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(buildData.buildPath))
                {
                    BaseCompress(Settings.OutputRoot + GetPathWithVars(_usedDate, buildData, 
                        Settings.DirPathForPostProcess));
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

            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

            switch (buildTargetGroup, isReleaseBuild)
            {
                case (BuildTargetGroup.Standalone, true):
                    buildOptions |= BuildOptions.CompressWithLz4;
                    PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Master);
                    Debug.Log(true);
                    break;
                case (BuildTargetGroup.Standalone, false):
                    buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);
                    PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.Mono2x);
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Debug);
                    Debug.Log(false);
                    break;

                case (BuildTargetGroup.Android, true):
                    buildOptions |= BuildOptions.CompressWithLz4;
                    PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.IL2CPP);
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Master);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
                    break;
                case (BuildTargetGroup.Android, false):
                    buildOptions &= ~(BuildOptions.CompressWithLz4 | BuildOptions.CompressWithLz4HC);
                    PlayerSettings.SetScriptingBackend(namedBuildTarget, ScriptingImplementation.Mono2x);
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Debug);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
                    break;

                case (BuildTargetGroup.WebGL, true):
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Master);
                    break;
                case (BuildTargetGroup.WebGL, false):
                    PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, Il2CppCompilerConfiguration.Debug);
                    break;
            }

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray(),
                locationPathName = buildPath,
                targetGroup = buildTargetGroup,
                target = buildTarget,
                options = buildOptions,
            };

            var scriptingDefineSymbolsOld = Settings.ScriptingDefineSymbolsDefault;
            var scriptingDefineSymbols = scriptingDefineSymbolsOld + ";" + AddonsUsed.GetAddonsUsed(addonsUsedType);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, scriptingDefineSymbols);

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, scriptingDefineSymbolsOld);

            var summary = buildReport!.summary;

            var report = $"Build {buildPlayerOptions.target} {summary.result.ToString()}\t ";
            report += $"Time: {summary.totalTime.ToString()}\t ";
            report += $"Size: {summary.totalSize / 1024 / 1024} Mb\n";

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log(report);
            }
            else
            {
                report += $"Error: {buildReport.SummarizeErrors()}";
                Debug.LogError(report);
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