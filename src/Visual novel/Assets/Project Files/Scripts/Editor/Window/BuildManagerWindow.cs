using System.Linq;
using Editor.BuildManager.Core;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Window
{
    public class BuildManagerWindow : EditorWindow
    {
        //Need Assets in path, cuz used by AssetDatabase.CreateAsset
        private const string SETTINGS_DEFAULT_PATH = "Assets/Editor/Setting/BuildSequences.asset";

        private const string SETTINGS_PATH_KEY = "BuildManagerWindow.SettingsPath";

        private static string _settingsPath;
        private static BuildManagerSettings _settings;

        private static bool _globalDataFoldout = true;

        private static bool _changelogFoldout = false;
        private static Vector2 _scrollPosChangelog = Vector2.zero;

        private static Vector2 _scrollPosSequence = Vector2.zero;
        private static bool _postBuildFoldout = false;

        private static ReorderableList _sequenceList;
        private static ReorderableList _buildList;

        [MenuItem("File/Build Manager", false, 205)]
        public static void ShowWindow()
        {
            _sequenceList = null;
            _buildList = null;
            GetWindow(typeof(BuildManagerWindow), false, "Builds", true);

            LoadSettings();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                LoadSettings();
            }
            
            DrawGlobalBuildData();
            
            if (!_changelogFoldout)
            {
                DrawBuildButtons();

                EditorGUILayout.Space(20);
                _scrollPosSequence = EditorGUILayout.BeginScrollView(_scrollPosSequence);

                DrawSequenceList();
                DrawSelectedSequenceData();

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Close changelog to acess build data", MessageType.Warning);
            }

            EditorUtility.SetDirty(_settings);
        }

        #region Main Drawers

        private void DrawGlobalBuildData()
        {
            _globalDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_globalDataFoldout, "Global data");
            if (_globalDataFoldout)
            {
                ++EditorGUI.indentLevel;

                //Versions
                PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
                PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);
                PlayerSettings.bundleVersion = EditorGUILayout.TextField("Version", PlayerSettings.bundleVersion);
                PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Android bundle version",
                    PlayerSettings.Android.bundleVersionCode);

                //Defines
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                _settings.scriptingDefineSymbols =
                    EditorGUILayout.TextField("Scripting Defines", _settings.scriptingDefineSymbols);
                if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
                {
                    var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                    var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                    PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, _settings.scriptingDefineSymbols);
                }

                EditorGUILayout.EndHorizontal();

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (_globalDataFoldout)
            {
                EditorGUILayout.Space(20);
            }
        }
        
        private static void DrawBuildButtons()
        {
            if ((_settings != null ? _settings.sequences?.Count ?? 0 : 0) == 0)
            {
                return;
            }

            var enabledSequence = _settings.sequences?.Count(sequence => sequence.isEnabled);

            if (enabledSequence == 0)
            {
                return;
            }

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.773f, 0.345098f, 0.345098f);

            EditorGUILayout.LabelField("Start build sequence(they red not becouse error, but becouse build " +
                                       "stuck your pc if you accidentaly press it)");
            EditorGUILayout.LabelField("Don't forget to manually download new version of polyglot localization " +
                                       "if you want to update it");

            EditorGUILayout.BeginHorizontal();
            if (_settings.sequences != null)
            {
                for (var i = 0; i < _settings.sequences.Count; ++i)
                {
                    var sequence = _settings.sequences[i];

                    if (i != 0 && i % 3 == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    if (sequence.isEnabled && GUILayout.Button($"Build {sequence.editorName}"))
                    {
                        BuildManager.Core.BuildManager.RunBuildSequnce(_settings, sequence);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = prevColor;
        }

        private static void DrawSequenceList()
        {
            if (_sequenceList == null)
            {
                PredefinedBuildConfigs.Init();
                _sequenceList =
                    BuildSequenceReordableList.Create(_settings.sequences, OnSequenceAdd, "Builds sequences");
                _sequenceList.onSelectCallback += OnSequenceSelectionChanged;
                _sequenceList.index = 0;
            }

            _sequenceList.DoLayoutList();

            if (0 <= _sequenceList.index && _sequenceList.index < _sequenceList.count)
            {
                var selected = _settings.sequences[_sequenceList.index];

                EditorGUILayout.BeginHorizontal();
                selected.scriptingDefineSymbolsOverride = EditorGUILayout.TextField("Defines sequence override",
                    selected.scriptingDefineSymbolsOverride);
                if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
                {
                    var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                    var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                    var concat = string.Concat(_settings.scriptingDefineSymbols, ";",
                        selected.scriptingDefineSymbolsOverride);
                    PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, concat);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawSelectedSequenceData()
        {
            EditorGUILayout.Space(20);

            if (_sequenceList.index < 0 || _sequenceList.index >= _settings.sequences.Count)
            {
                _buildList = null;
                return;
            }

            if (_buildList == null)
            {
                _buildList = BuildDataReordableList.Create(_settings.sequences[_sequenceList.index].builds, OnBuildAdd,
                    "Builds");
                _buildList.onSelectCallback += OnBuildSelectionChanged;
                _buildList.index = 0;
            }

            _buildList.DoLayoutList();

            if (_buildList.index < 0 || _buildList.index >= _settings.sequences[_sequenceList.index].builds.Count)
            {
                return;
            }

            var selected = _settings.sequences[_sequenceList.index].builds[_buildList.index];

            var obj = new SerializedObject(_settings);

            selected.isPassbyBuild = EditorGUILayout.Toggle("Is Passby build", selected.isPassbyBuild);
            selected.isReleaseBuild = EditorGUILayout.Toggle("Is Release build", selected.isReleaseBuild);

            EditorGUILayout.Space(20);
            selected.outputRoot = EditorGUILayout.TextField("Output root", selected.outputRoot);
            selected.middlePath = EditorGUILayout.TextField("Middle path", selected.middlePath);
            selected.dirPathForPostProcess = EditorGUILayout.TextField("Dir path", selected.dirPathForPostProcess);

            EditorGUILayout.BeginHorizontal();
            selected.scriptingDefineSymbolsOverride =
                EditorGUILayout.TextField("Defines build override", selected.scriptingDefineSymbolsOverride);
            if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                var concat = string.Concat(_settings.scriptingDefineSymbols, ";",
                    _settings.sequences[_sequenceList.index].scriptingDefineSymbolsOverride, ";",
                    selected.scriptingDefineSymbolsOverride);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, concat);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #endregion

        #region Loaders

        private static void LoadSettings()
        {
            _settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");
            _settings = null;

            //Find path. Try to load settings
            if (!string.IsNullOrEmpty(_settingsPath))
            {
                _settings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(_settingsPath);
                if (_settings == null)
                {
                    _settingsPath = null;
                }
            }

            //No path, or cant locate asset at path. Try to find settings in assets.
            if (string.IsNullOrEmpty(_settingsPath))
            {
                var guids = AssetDatabase.FindAssets("t:BuildManagerSettings", new[] { "Assets" });
                if (guids.Length >= 2)
                {
                    Debug.LogError("[BuildManagerWindow]. 2+ BuildManagerSettings exist. " +
                                   "Consider on using only 1 setting. The first one will be used.");
                }

                if (guids.Length != 0)
                {
                    _settingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    PlayerPrefs.SetString(SETTINGS_PATH_KEY, _settingsPath);
                    _settings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(_settingsPath);
                }
            }

            //Cant find settings. Create new
            if (_settings == null)
            {
                var defaultSettings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(
                    AssetDatabase.GUIDToAssetPath(
                        AssetDatabase.FindAssets("t:BuildManagerSettings", new[] { "Packages" })[0]));

                _settings = (BuildManagerSettings)CreateInstance(typeof(BuildManagerSettings));
                _settings.CloneInto(defaultSettings);

                AssetDatabase.CreateAsset(_settings, SETTINGS_DEFAULT_PATH);
                _settingsPath = SETTINGS_DEFAULT_PATH;
                PlayerPrefs.SetString(SETTINGS_PATH_KEY, SETTINGS_DEFAULT_PATH);
            }
        }

        #endregion

        #region Callbacks

        private static void OnSequenceSelectionChanged(ReorderableList list)
        {
            _buildList = null;
        }

        private static void OnBuildSelectionChanged(ReorderableList list)
        {
        }

        private static void OnSequenceAdd(object target)
        {
            _settings.sequences.Add((target as BuildSequence)?.Clone() as BuildSequence);
        }

        private static void OnBuildAdd(object target)
        {
            _settings.sequences[_sequenceList.index].builds.Add((target as BuildData)?.Clone() as BuildData);
        }

        #endregion

        #region Helpers

        private void GuiLine(int iHeight = 1)
        {
            var rect = EditorGUILayout.GetControlRect(false, iHeight);
            rect.height = iHeight;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        #endregion
    }
}