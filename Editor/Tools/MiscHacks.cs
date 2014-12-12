using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UnityHelpers
{
    public class MiscHacks
    {
        public static void OpenSpriteEditor(string texturePath)
        {
            // Lookup the Editor assembly
            if (typeof(EditorGUIUtility).Assembly == null)
                throw new Exception("Cannot find the EditorGUIUtility Assembly therefore cannot search for SpriteEditorWindow");

            // Look for the internal class
            var type = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.SpriteEditorWindow");
            if (type == null) 
                throw new Exception("Cannot find the SpriteEditorWindow class!");

            // Get the call to open the window
            var openMethod = type.GetMethod("GetWindow");
            if (openMethod == null) 
                throw new Exception("Cannot find the GetWindow method");

            // Select the texture
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(texturePath);

            // Finally open it
            openMethod.Invoke(null, null);
        }
    }
}
