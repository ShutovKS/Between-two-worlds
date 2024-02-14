using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BuildsManager.Data;
using BuildsManager.Utility;
using Ionic.Zip;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BuildsManager.Core
{
    public static class MainManager
    {
        public static GeneralBuildData GeneralBuildData { get; private set; }

        private static DateTime _usedDate;
        private static string _shownPath;

        public static void RunBuild()
        {
            if (GeneralBuildData == null || GeneralBuildData.builds.Count == 0)
            {
                Debug.LogError("No data available");
                return;
            }

            var startTime = DateTime.Now;
            _usedDate = DateTime.Now;


            BuildAll();

            for (byte i = 0; i < GeneralBuildData.builds.Count; ++i)
            {
                var buildData = GeneralBuildData.builds[i];

                if (!buildData.isCompress || !buildData.isEnabled || buildData.targetGroup == BuildTargetGroup.Android)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(buildData.buildPath))
                {
                    BaseCompress(GeneralBuildData.outputRoot + ConvertToStrings.GetPathWithVars(_usedDate,
                        buildData, GeneralBuildData.dirPathForPostProcess));
                    return;
                }

                Debug.LogWarning("Can't find build for " + ConvertToStrings.GetBuildTargetExecutable(buildData.target));
            }

            Debug.Log($@"End building all. Elapsed time: {(DateTime.Now - startTime).ToString()}");

#if UNITY_EDITOR_WIN
            if (string.IsNullOrEmpty(_shownPath) == false)
            {
                Explorer.ShowExplorer(_shownPath);
            }
#endif
        }

        #region Loading Data

        public static void LoadSettings()
        {
            const string SETTINGS_DEFAULT_PATH = "Assets/Plugins/BuildsManager/Settings/GeneralBuildData.asset";
            const string SETTINGS_PATH_KEY = "BuildManagerWindow.SettingsPath";

            var settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");

            if (!string.IsNullOrEmpty(settingsPath))
            {
                GeneralBuildData = AssetDatabase.LoadAssetAtPath<GeneralBuildData>(settingsPath);
                if (GeneralBuildData == null)
                {
                    settingsPath = null;
                }
            }

            if (string.IsNullOrEmpty(settingsPath))
            {
                var guids = AssetDatabase.FindAssets("t:GeneralBuildData", new[] { "Assets" });
                if (guids.Length >= 2)
                {
                    Debug.LogError("2+ GeneralBuildData exist. Consider on using only 1 setting. " +
                                   "The first one will be used.");
                }

                if (guids.Length != 0)
                {
                    settingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    PlayerPrefs.SetString(SETTINGS_PATH_KEY, settingsPath);
                    GeneralBuildData = AssetDatabase.LoadAssetAtPath<GeneralBuildData>(settingsPath);
                }
            }

            if (GeneralBuildData == null)
            {
                GeneralBuildData = (GeneralBuildData)ScriptableObject.CreateInstance(typeof(GeneralBuildData));
                AssetDatabase.CreateAsset(GeneralBuildData, SETTINGS_DEFAULT_PATH);
                PlayerPrefs.SetString(SETTINGS_PATH_KEY, SETTINGS_DEFAULT_PATH);
            }
        }

        #endregion

        private static void BuildAll()
        {
            var targetBeforeStart = EditorUserBuildSettings.activeBuildTarget;
            var targetGroupBeforeStart = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTargetStart = NamedBuildTarget.FromBuildTargetGroup(targetGroupBeforeStart);
            var definesBeforeStart = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

            _shownPath = GeneralBuildData.outputRoot;

            for (byte i = 0; i < GeneralBuildData.builds.Count; ++i)
            {
                var buildData = GeneralBuildData.builds[i];

                if (buildData.isEnabled == false)
                {
                    continue;
                }

                buildData.buildPath = GeneralBuildData.outputRoot + ConvertToStrings.GetPathWithVars(_usedDate,
                    buildData, GeneralBuildData.middlePath);

                var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path)
                    .ToArray();

                var buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = buildData.buildPath,
                    targetGroup = buildData.targetGroup,
                    target = buildData.target,
                    options = buildData.options,
                };

                BaseBuild(buildPlayerOptions, buildData.addonsUsed, GeneralBuildData.isReleaseBuild);
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroupBeforeStart, targetBeforeStart);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, definesBeforeStart);
        }

        #region Base methods

        private static void BaseBuild(BuildPlayerOptions buildPlayerOptions, AddonsUsedType addonsUsedType,
            bool isReleaseBuild)
        {
            var targetGroup = buildPlayerOptions.targetGroup;

            if (targetGroup == BuildTargetGroup.Android && PlayerSettings.Android.useCustomKeystore &&
                string.IsNullOrEmpty(PlayerSettings.Android.keyaliasPass))
            {
                PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = "keystore";
            }

            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            EditorUserBuildSettings.SwitchActiveBuildTarget(namedBuildTarget, buildPlayerOptions.target);

            var implementation = isReleaseBuild ? ScriptingImplementation.IL2CPP : ScriptingImplementation.Mono2x;
            PlayerSettings.SetScriptingBackend(namedBuildTarget, implementation);

            var configuration = isReleaseBuild ? Il2CppCompilerConfiguration.Master : Il2CppCompilerConfiguration.Debug;
            PlayerSettings.SetIl2CppCompilerConfiguration(namedBuildTarget, configuration);

            if (targetGroup == BuildTargetGroup.Android)
            {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            }

            var preBuildDefines = GeneralBuildData.generalScriptingDefineSymbols;
            var buildDefines = preBuildDefines + ";" + AddonsUsed.GetScriptingDefineSymbols(addonsUsedType);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, buildDefines);
            
            AssetDatabase.Refresh();

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, preBuildDefines);

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

            long uncompressedSize = 0;
            long compressedSize = 0;
            foreach (var e in zip.Entries)
            {
                uncompressedSize += e.UncompressedSize;
                compressedSize += e.CompressedSize;
            }

            Debug.Log($"Make {dirPath}.zip.  \t " +
                      $"Time: {(DateTime.Now - startTime).ToString()}  \t " +
                      $"Size: {uncompressedSize / 1048576} Mb - {compressedSize / 1048576} Mb");
        }

        #endregion
    }
}