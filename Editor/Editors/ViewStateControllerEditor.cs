using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityHelpers;
using UnityHelpers.View;

namespace UnityHelpers
{
    [CustomEditor(typeof(ViewStateController))]
    public class ViewStateControllerEditor : UnityEditor.Editor
    {
        private ReorderableList list;
        private ViewStateController controller;
        private GUIContent visibleOnIcon;
        private GUIContent visibleOffIcon;

        void Awake()
        {
            controller = (ViewStateController)target;

            visibleOnIcon = EditorGUIUtility.IconContent("animationvisibilitytoggleon");
            visibleOffIcon = EditorGUIUtility.IconContent("animationvisibilitytoggleoff");

            list = new ReorderableList(controller.states, typeof(GameObject), true, true, true, true);

            ReorderableListUtil.SetColumns(list, new List<ReorderableListColumn<GameObject>>
            {
                new ReorderableListColumn<GameObject> {
                    Name = "Id",
                    Width = 20,
                    ItemRenderer = (GameObject state, Rect rect, int index, bool isActive, bool isFocused) => {
                        EditorGUI.LabelField(rect, "" + index);
                    }
                },
                new ReorderableListColumn<GameObject> {
                    Name = "State",
                    WidthRatio = 1,
                    ItemRenderer = (GameObject state, Rect rect, int index, bool isActive, bool isFocused) => {
                        controller.states[index] = (GameObject)EditorGUI.ObjectField(rect, state, typeof(GameObject), true);
                    }
                },
                new ReorderableListColumn<GameObject> {
                    Name = "",
                    Width = 10,
                    ItemRenderer = (GameObject state, Rect rect, int index, bool isActive, bool isFocused) => {
                        EditorGUI.LabelField(rect, "");
                    }
                },
                new ReorderableListColumn<GameObject> {
                    Name = "",
                    Width = 15,
                    ItemRenderer = (GameObject state, Rect rect, int index, bool isActive, bool isFocused) => {
                        var content = state.activeSelf ? visibleOnIcon : visibleOffIcon;
                        var r = new Rect(rect);
                        r.y += 4;
                        var b = GUI.Toggle(r, state.activeSelf, content, GUIStyle.none);
                        if (b != state.activeSelf && b) controller.SetState(state);
                    }
                }
            });
        }


        public override void OnInspectorGUI()
        {
            if (controller != null && list !=null)
            {
                EditorGUILayout.LabelField("States");
                list.list = controller.states;
                list.DoLayoutList();
            }

            //serializedObject.Update();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("stateChanged"));
            //serializedObject.ApplyModifiedProperties();            

            ViewStateControllerHierachyUtil.UpdateStatesList();
        }
    }
}
