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
        private static bool _sequenceListFoldout = true;

        private static Vector2 _scrollPosSequence = Vector2.zero;

        private static ReorderableList _sequenceList;
        private static ReorderableList _buildList;

        [MenuItem("File/Builds Manager", false, 205)]
        public static void ShowWindow()
        {
            _sequenceList = null;
            _buildList = null;

            GetWindow(typeof(BuildManagerWindow), false, "Builds Manager", true);

            LoadSettings();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            DrawGlobalBuildData();

            DrawBuildButtons();
            
            _scrollPosSequence = EditorGUILayout.BeginScrollView(_scrollPosSequence);

            DrawSequenceList();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawSelectedSequenceData();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawPathData();

            EditorGUILayout.EndScrollView();

            EditorUtility.SetDirty(_settings);
        }

        #region Main Drawers

        private void DrawGlobalBuildData()
        {
            _globalDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_globalDataFoldout, "Глобальные данные");

            if (_globalDataFoldout)
            {
                ++EditorGUI.indentLevel;

                PlayerSettings.companyName = EditorGUILayout.TextField("Название компании", PlayerSettings.companyName);
                PlayerSettings.productName = EditorGUILayout.TextField("Название проекта", PlayerSettings.productName);
                PlayerSettings.bundleVersion = EditorGUILayout.TextField("Версия", PlayerSettings.bundleVersion);

                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField("Для Android");
                PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Версия пакета Android",
                    PlayerSettings.Android.bundleVersionCode);
                PlayerSettings.Android.keystorePass =
                    EditorGUILayout.TextField("Android keystore pass", PlayerSettings.Android.keystorePass);
                PlayerSettings.Android.keyaliasPass =
                    EditorGUILayout.TextField("Android keyalias pass", PlayerSettings.Android.keyaliasPass);

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
            if (_settings == null || _settings.sequences == null || _settings.sequences.Count == 0)
            {
                return;
            }

            var enabledSequence = _settings.sequences?.Where(sequence => sequence.isEnabled).ToArray();

            if (enabledSequence?.Length == 0)
            {
                return;
            }

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.773f, 0.345098f, 0.345098f);

            EditorGUILayout.BeginHorizontal();

            for (var i = 0; i < enabledSequence!.Length; ++i)
            {
                var sequence = enabledSequence[i];

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

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = prevColor;
        }

        private static void DrawSequenceList()
        {
            ++EditorGUI.indentLevel;
            
            _sequenceListFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_sequenceListFoldout, "Настройки кнопок для сборки");
            
            if (_sequenceListFoldout && _settings != null)
            {
                PredefinedBuildConfigs.Initialize();
                _sequenceList = BuildSequenceReordableList.Create(_settings.sequences, OnSequenceAdd,
                    "Список кнопок для сборки");
                _sequenceList.onSelectCallback += OnSequenceSelectionChanged;
                _sequenceList.index = 0;
            }

            if (_sequenceListFoldout)
            {
                _sequenceList.DoLayoutList();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            --EditorGUI.indentLevel;
        }

        private static void DrawSelectedSequenceData()
        {
            if (_sequenceList.index < 0 || _sequenceList.index >= _settings.sequences.Count)
            {
                _buildList = null;
                return;
            }

            if (_buildList == null)
            {
                _buildList = BuildDataReordableList.Create(_settings.sequences[_sequenceList.index].builds, OnBuildAdd, "Builds");
                _buildList.onSelectCallback += OnBuildSelectionChanged;
                _buildList.index = 0;
            }

            _buildList.DoLayoutList();

            if (_buildList.index < 0 || _buildList.index >= _settings.sequences[_sequenceList.index].builds.Count)
            {
                return;
            }

            var selected = _settings.sequences[_sequenceList.index].builds[_buildList.index];

            selected.isPassbyBuild = EditorGUILayout.Toggle("Is Passby build", selected.isPassbyBuild);
            selected.isReleaseBuild = EditorGUILayout.Toggle("Is Release build", selected.isReleaseBuild);

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawPathData()
        {
            if (_buildList.index < 0 || _buildList.index >= _settings.sequences[_sequenceList.index].builds.Count)
            {
                return;
            }

            var selected = _settings.sequences[_sequenceList.index].builds[_buildList.index];

            selected.outputRoot = EditorGUILayout.TextField("Output root", selected.outputRoot);
            selected.middlePath = EditorGUILayout.TextField("Middle path", selected.middlePath);
            selected.dirPathForPostProcess = EditorGUILayout.TextField("Dir path", selected.dirPathForPostProcess);
        }

        #endregion

        #region Loaders

        private static void LoadSettings()
        {
            _settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");
            _settings = null;

            // Если путь сохранен, попробуйте загрузить настройки
            if (!string.IsNullOrEmpty(_settingsPath))
            {
                _settings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(_settingsPath);
                if (_settings == null)
                {
                    _settingsPath = null;
                }
            }

            // Нет пути или невозможно найти актив по пути. Попробуйте найти настройки в активах.
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

            // Если не удалось найти настройки, создаем новые
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
    }
}