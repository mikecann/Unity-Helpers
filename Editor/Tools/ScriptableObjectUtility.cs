using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Libraries.UnityHelpers.Editor.Tools
{
    public class ScriptableObjectUtility
    {
        //[MenuItem("UnityHelpers/Create Asset")]
        //public static void CreateAsset()
        //{
        //    EditorGUIUtility.ShowObjectPicker<MonoScript>(null, false, "", 1234);
        //    //ScriptableObjectUtility.CreateAsset<UnityHelpersSettings>();
        //}

        /// <summary>    
        /// This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// Borrowed from: http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
        /// </summary>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
