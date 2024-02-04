using System.Collections.Generic;
using Editor.BuildManager.Core;
using UnityEditor;
using UnityEngine;

namespace Editor.Window
{
    public class BuildSequenceReordableList
    {
        public static UnityEditorInternal.ReorderableList Create(List<BuildSequence> configsList,
            GenericMenu.MenuFunction2 menuItemHandler, string header)
        {
            var reorderableList = new UnityEditorInternal.ReorderableList(
                configsList, typeof(BuildSequence), true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight + 4,
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, header); },
                drawElementCallback = (position, index, isActive, isFocused) =>
                {
                    const float ENABLED_WIDTH = 15f;
                    const float SPACE = 10f;
                    const float MIN_NAME_WIDTH = 100f;

                    var sequence = configsList[index];
                    var nameWidth = position.width - SPACE * 3 - ENABLED_WIDTH;
                    if (nameWidth < MIN_NAME_WIDTH)
                    {
                        nameWidth = MIN_NAME_WIDTH;
                    }

                    position.y += 2;
                    position.height -= 4;

                    position.x += SPACE;
                    position.width = ENABLED_WIDTH;
                    sequence.isEnabled = EditorGUI.Toggle(position, sequence.isEnabled);
                    EditorGUI.BeginDisabledGroup(!sequence.isEnabled);

                    position.x += position.width + SPACE;
                    position.width = nameWidth;
                    sequence.editorName = EditorGUI.TextField(position, sequence.editorName);

                    EditorGUI.EndDisabledGroup();
                },
                onAddDropdownCallback = (buttonRect, list) =>
                {
                    var menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Custom"), false, menuItemHandler,
                        new BuildSequence("Custom", new BuildData()));
                    menu.AddSeparator("");

                    var label = $"{PredefinedBuildConfigs.TestingSequence.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler, PredefinedBuildConfigs.TestingSequence);

                    label = $"{PredefinedBuildConfigs.TestingSequenceZip.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler,
                        PredefinedBuildConfigs.TestingSequenceZip);


                    menu.AddSeparator("");
                    label = $"{PredefinedBuildConfigs.ReleaseLocalSequence.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler,
                        PredefinedBuildConfigs.ReleaseLocalSequence);

                    label = $"{PredefinedBuildConfigs.ReleaseLocalZipSequence.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler,
                        PredefinedBuildConfigs.ReleaseLocalZipSequence);

                    label = $"{PredefinedBuildConfigs.ReleaseFullSequence.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler,
                        PredefinedBuildConfigs.ReleaseFullSequence);


                    menu.AddSeparator("");
                    label = $"{PredefinedBuildConfigs.PassbySequence.editorName}";
                    menu.AddItem(new GUIContent(label), false, menuItemHandler, PredefinedBuildConfigs.PassbySequence);


                    menu.ShowAsContext();
                }
            };

            return reorderableList;
        }
    }
}