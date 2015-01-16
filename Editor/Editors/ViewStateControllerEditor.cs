using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityHelpers.View;

namespace Assets.Libraries.UnityHelpers.Editor.Editors
{
    [CustomEditor(typeof(ViewStateController))]
    public class ViewStateControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var controller = (ViewStateController)target;

            if (controller.states!=null && controller.states.Count > 0)
            {
                var options = controller.states.Select(s => s==null?"None":s.name).ToArray();
                var newSelectedIndex = EditorGUILayout.Popup("Current State", controller.CurrentStateIndex, options);
                if (newSelectedIndex != controller.CurrentStateIndex)
                {
                    controller.SetState(newSelectedIndex);
                    EditorUtility.SetDirty(controller);
                }                    
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("states"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
