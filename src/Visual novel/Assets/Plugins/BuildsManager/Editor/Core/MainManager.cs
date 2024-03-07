using System;
using System.Collections.Generic;
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

        public static void RunBuild(BuildData buildData = null)
        {
            if (GeneralBuildData == null || GeneralBuildData.builds.Count == 0)
            {
                Debug.LogError("No data available");
                return;
            }

            var startTime = DateTime.Now;
            _usedDate = DateTime.Now;

            if (buildData == null)
            {
                BuildAll();
                CompressAll();
            }
            else
            {
                BuildOne(buildData);
            }

            Debug.Log($@"End building all. Elapsed time: {(DateTime.Now - startTime).ToString()}");

#if UNITY_EDITOR_WIN
            if (string.IsNullOrEmpty(GeneralBuildData.outputRoot) == false)
            {
                Explorer.ShowExplorer(GeneralBuildData.outputRoot);
            }
#endif
        }

        private static void BuildAll()
        {
            var targetBeforeStart = EditorUserBuildSettings.activeBuildTarget;
            var targetGroupBeforeStart = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTargetStart = NamedBuildTarget.FromBuildTargetGroup(targetGroupBeforeStart);
            var definesBeforeStart = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

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
                    target = buildData.target,
                    options = buildData.options,
                };

                PreBuild(buildData);

                BaseBuild(buildPlayerOptions, buildData.addonsUsed, GeneralBuildData.isReleaseBuild);

                PostBuild(buildData);
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroupBeforeStart, targetBeforeStart);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, definesBeforeStart);
        }

        private static void BuildOne(BuildData buildData)
        {
            var targetBeforeStart = EditorUserBuildSettings.activeBuildTarget;
            var targetGroupBeforeStart = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTargetStart = NamedBuildTarget.FromBuildTargetGroup(targetGroupBeforeStart);
            var definesBeforeStart = PlayerSettings.GetScriptingDefineSymbols(namedBuildTargetStart);

            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = GeneralBuildData.outputRoot + ConvertToStrings.GetPathWithVars(_usedDate,
                    buildData, GeneralBuildData.middlePath),
                target = buildData.target,
                options = buildData.options,
            };

            PreBuild(buildData);

            BaseBuild(buildPlayerOptions, buildData.addonsUsed, GeneralBuildData.isReleaseBuild);

            PostBuild(buildData);

            if (buildData.isCompress)
            {
                BaseCompress(GeneralBuildData.outputRoot + ConvertToStrings.GetPathWithVars(_usedDate, buildData,
                    GeneralBuildData.dirPathForPostProcess));
            }

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroupBeforeStart, targetBeforeStart);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTargetStart, definesBeforeStart);
        }

        private static void CompressAll()
        {
            for (byte i = 0; i < GeneralBuildData.builds.Count; ++i)
            {
                var buildData = GeneralBuildData.builds[i];

                if (!buildData.isCompress || !buildData.isEnabled || buildData.target == BuildTarget.Android)
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
        }

        #region Base methods

        private static void BaseBuild(BuildPlayerOptions buildPlayerOptions,
            IEnumerable<AddonUsedInformation> addonsUsed,
            bool isReleaseBuild)
        {
            var targetGroup = buildPlayerOptions.target.ToBuildTargetGroup();

            if (buildPlayerOptions.target == BuildTarget.Android && PlayerSettings.Android.useCustomKeystore &&
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

            var addonsUsedDetailed = new List<AddonUsedDetailed>();

            foreach (var addonUsed in addonsUsed)
            {
                if (!addonUsed.IsUsed)
                {
                    continue;
                }

                foreach (var addonUsedDetailed in GeneralBuildData.addonsUsedData.AddonsUsed.Where(
                             addonUsedDetailed => addonUsedDetailed.Name == addonUsed.Name))
                {
                    addonsUsedDetailed.Add(addonUsedDetailed);
                    break;
                }
            }

            var buildDefines = addonsUsedDetailed.Aggregate(preBuildDefines,
                (current, addonUsed) => current + ";" + string.Join(";", addonUsed.Defines));
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

        private static void PreBuild(BuildData buildData)
        {
            var buildPath = buildData.buildPath;

            if (!string.IsNullOrEmpty(buildPath))
            {
                DestroyBuildDirectory(buildPath);
            }
        }

        private static void PostBuild(BuildData buildData)
        {
            var buildPath = buildData.buildPath;

            if (GeneralBuildData.isReleaseBuild && !string.IsNullOrEmpty(buildPath))
            {
                DestroyIL2CPPJunk(buildPath);
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

        #region Loading Data

        public static GeneralBuildData LoadSettings()
        {
            const string SETTINGS_DEFAULT_PATH = "Assets/Plugins/BuildsManager/Settings/GeneralBuildData.asset";
            const string SETTINGS_PATH_KEY = "BuildManagerWindow.SettingsPath";

            var settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");
            GeneralBuildData = LoadAssetAtPath<GeneralBuildData>(settingsPath, SETTINGS_DEFAULT_PATH);
            PlayerPrefs.SetString(SETTINGS_PATH_KEY, AssetDatabase.GetAssetPath(GeneralBuildData));

            GeneralBuildData.addonsUsedData = LoadAddonsUsedData();

            return GeneralBuildData;
        }

        public static AddonsUsedData LoadAddonsUsedData()
        {
            const string ADDONS_USED_DEFAULT_PATH = "Assets/Plugins/BuildsManager/Settings/AddonsUsedData.asset";
            const string ADDONS_USED_PATH_KEY = "BuildManagerWindow.AddonsUsedPath";

            var addonsUsedPath = PlayerPrefs.GetString(ADDONS_USED_PATH_KEY, "");
            var addonsUsedData = LoadAssetAtPath<AddonsUsedData>(addonsUsedPath, ADDONS_USED_DEFAULT_PATH);
            PlayerPrefs.SetString(ADDONS_USED_PATH_KEY, AssetDatabase.GetAssetPath(addonsUsedData));


            return LoadAssetAtPath<AddonsUsedData>(addonsUsedPath, ADDONS_USED_DEFAULT_PATH);
        }

        private static T LoadAssetAtPath<T>(string path, string defaultPath) where T : ScriptableObject
        {
            T asset = null;
            if (!string.IsNullOrEmpty(path))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null) path = null;
            }

            if (string.IsNullOrEmpty(path))
            {
                var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets" });
                if (guids.Length >= 2)
                    Debug.LogError(
                        $"2+ {typeof(T).Name} exist. Consider using only 1 setting. The first one will be used.");

                if (guids.Length != 0)
                {
                    path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    asset = AssetDatabase.LoadAssetAtPath<T>(path);
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, defaultPath);
                path = defaultPath;
            }

            return asset;
        }

        #endregion

        #region Oth

        public static void OpenAddonsUsedData()
        {
            var path = AssetDatabase.GetAssetPath(GeneralBuildData.addonsUsedData);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Can't find AddonsUsedData");
                return;
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = GeneralBuildData.addonsUsedData;
        }

        private static void DestroyIL2CPPJunk(string buildPath)
        {
            var buildRootPath = Path.GetDirectoryName(buildPath);
            var dirs = Directory.GetDirectories(buildRootPath!);
            var il2CPPDirs = dirs.Where(s => s.Contains("BackUpThisFolder_ButDontShipItWithYourGame"));
            foreach (var dir in il2CPPDirs)
            {
                Directory.Delete(dir, true);
            }

            var pathToUnityCrashHandler = Path.Combine(buildRootPath, "UnityCrashHandler64.exe");
            File.Delete(pathToUnityCrashHandler);
        }

        private static void DestroyBuildDirectory(string buildPath)
        {
            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, true);
            }
            else if (File.Exists(buildPath))
            {
                var dirs = Directory.GetParent(buildPath!)?.FullName;
                if (dirs != null)
                {
                    Directory.Delete(dirs, true);
                }
            }
        }

        #endregion
    }
}