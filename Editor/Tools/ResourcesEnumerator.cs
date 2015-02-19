using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityHelpers
{
    public class ResourcesEnumerator : EditorWindow
    {
        private const string FileTemplate = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace &&NAMESPACE&&
{
&&CLASSES&&
}
";

        private const string ClassTemplate = @"public class &&CLASS-NAME&&
{
&&CLASS-CONTENTS&&
}";

        public string outputPath;
        public string pathToResourcesFolder;
        public string outputNamespace;

        [MenuItem("Unity Helpers/Enumerate Resources")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:		
            var window = new ResourcesEnumerator();
            window.title = "Enumerate Resources";
            window.outputPath = EditorPrefs.GetString("UnityHelpers_GenerateResources_pathToSaveTo");
            window.pathToResourcesFolder = EditorPrefs.GetString("UnityHelpers_GenerateResources_pathToResourcesFolder");
            window.outputNamespace = EditorPrefs.GetString("UnityHelpers_GenerateResources_outputNamespace");
            if (String.IsNullOrEmpty(window.outputPath)) window.outputPath = "Scripts/GameResources.cs";
            if (String.IsNullOrEmpty(window.pathToResourcesFolder)) window.pathToResourcesFolder = "Resources";
            if (String.IsNullOrEmpty(window.outputNamespace)) window.outputNamespace = "UnityHelpers";
                        
            window.ShowAuxWindow();
        }

        void OnGUI()
        {
            pathToResourcesFolder = EditorGUILayout.TextField("Resources Folder", pathToResourcesFolder);
            outputPath = EditorGUILayout.TextField("Output Folder", outputPath);
            outputNamespace = EditorGUILayout.TextField("Output Namespace", outputNamespace);

            if (GUILayout.Button("Enumerate", GUILayout.Height(50)))
            {
                var classes = new List<string>();
                var c = GetClass("Assets/" + pathToResourcesFolder, Path.GetFileNameWithoutExtension(outputPath), 0);
                c = String.Join("\n", c.Split('\n').ToList().ConvertAll(l => "\t" + l).ToArray());
                File.WriteAllText(Application.dataPath + "/" + outputPath, FileTemplate.Replace("&&CLASSES&&", c).Replace("&&NAMESPACE&&", outputNamespace));
                AssetDatabase.Refresh();
                Close();
            }

            // Save the settings
            EditorPrefs.SetString("UnityHelpers_GenerateResources_pathToSaveTo", outputPath);
            EditorPrefs.SetString("UnityHelpers_GenerateResources_pathToResourcesFolder", pathToResourcesFolder);
            EditorPrefs.SetString("UnityHelpers_GenerateResources_outputNamespace", outputNamespace);
        }

        private string GetClass(string path, string className, int depth)
        {
            var assetPaths = new List<string>(AssetDatabase.FindAssets("*.*", new[] { "Assets/" + pathToResourcesFolder })).ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            var classes = new List<string>();
            var files = new List<string>();

            foreach (var assetPath in assetPaths)
            {
                // Is it not the immediate directory then continue
                if (Path.GetDirectoryName(assetPath).Replace(path,"") != "") continue;

                // If its a directory then recurse
                if (!Path.HasExtension(assetPath))
                {
                    var dname = assetPath.Replace(path, "").Replace("/", "").Replace(" ", "");
                    classes.Add(GetClass(assetPath, dname, depth+1));
                }
                else files.Add(assetPath);
            }

            var lines = new List<string>();

            lines.AddRange(classes);
            lines.AddRange(files.Distinct().ToList().ConvertAll(f =>
            {
                var fileName = Path.GetFileName(f);
                var fileExtension = Path.GetExtension(f);
                var varName = Path.GetFileNameWithoutExtension(f).Replace(" ", "");
                if (Char.IsNumber(varName[0])) varName = "_" + varName;
                varName = varName.Replace("-", "Minus");
                var strPath = Path.GetDirectoryName(f).Replace("Assets/" + pathToResourcesFolder, "") + "/" + Path.GetFileNameWithoutExtension(f);
                if (strPath[0] == '/') strPath = strPath.Substring(1);
                if (fileExtension == ".unity") strPath = Path.GetFileNameWithoutExtension(f);
                return "\tpublic const string " + varName + " = \"" + strPath + "\";";                
            }));

            // Add this path
            lines.Add("\tpublic const string _Path = \"" + path.Replace("Assets/Resources/","") + "\";");

            var contents = String.Join("\n", lines.ToArray());

            var tabs = "";
            for(var i=0; i<Math.Max(depth,0); i++) tabs += "\t";
            var c = ClassTemplate.Replace("&&CLASS-NAME&&", className).Replace("&&CLASS-CONTENTS&&", contents);
            return String.Join("\n", c.Split('\n').ToList().ConvertAll(l => tabs + l).ToArray());
        }
    }
}
