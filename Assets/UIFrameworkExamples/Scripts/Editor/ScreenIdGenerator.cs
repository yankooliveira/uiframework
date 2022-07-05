using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace deVoid.UIFramework.Editor
{
    /// <summary>
    /// It's way less error prone to have constants for your UI instead of directly typing in ids.
    /// This utility searches through a folder for Screen prefabs and creates a class with Screen Id constants
    /// based on their names. That way you can do eg:
    ///
    /// UIFrame.OpenWindow(ScreenIds.ItsAWindow);
    ///
    /// instead of wondering for ages why
    ///
    /// UIFrame.OpenWindow("ltsAWindow");
    ///
    /// isn't working. It isn't a perfect solution, however: if you rename prefabs, you may have to refactor part
    /// of your code. You could also manually define these and use an asset postprocessor to validate it and warn
    /// if the data is stale, or just do it manually. The solution below is my current favourite local maximum.
    /// </summary>
    public class ScreenIdProcessor : AssetPostprocessor
    {
        private const string UIPrefabFolder = "Assets/UIFrameworkExamples/Prefabs/Screens";
        private const string UIIdScriptFolder = "Assets/UIFrameworkExamples/Scripts";
        private const string ScreenIdScriptName = "ScreenIds";
        private const string ScreenIdScriptNamespace = "deVoid.UIFramework.Examples";

        [MenuItem("Assets/Create/deVoid UI/Re-generate UI ScreenIds")]
        public static void RegenerateScreenIdsAndRefresh() {
            RegenerateScreenIds(true);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string str in importedAssets) {
                if (str.Contains(UIPrefabFolder)) {
                    RegenerateScreenIds(true);
                    return;
                }
            }

            foreach (string str in deletedAssets) {
                if (str.Contains(UIPrefabFolder)) {
                    RegenerateScreenIds(true);
                    return;
                }
            }

            for (int i = 0; i < movedAssets.Length; i++) {
                if (movedAssets[i].Contains(UIPrefabFolder)
                    || movedFromAssetPaths[i].Contains(UIPrefabFolder)) {
                    RegenerateScreenIds(true);
                    return;
                }
            }
        }

        public static void RegenerateScreenIds(bool refreshAssetDatabase) {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            var assets = AssetDatabase.FindAssets("t:prefab", new[] {UIPrefabFolder});
            foreach (var asset in assets) {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var screenController = go.GetComponent<IUIScreenController>();
                var name = go.name.Replace(" ", string.Empty);
                if (screenController != null) {
                    if (paths.ContainsKey(name)) {
                        Debug.LogError(
                            string.Format(
                                "You have multiple screen prefabs with the same name: {0}! Locations: (1){1}, (2){2}",
                                name, paths[name], path));
                    }
                    else {
                        paths.Add(name, path);
                        Debug.Log(string.Format("Registering {0} as {1}", path, name));
                    }
                }
            }

            var scripts = AssetDatabase.FindAssets(string.Format("t:script {0}", ScreenIdScriptName), new[] {UIIdScriptFolder});
            if (scripts.Length > 0) {
                string filePath = AssetDatabase.GUIDToAssetPath(scripts[0]);
                WriteIdClass(paths, filePath);
                if (refreshAssetDatabase) {
                    AssetDatabase.Refresh();
                }
            }
            else {
                Debug.LogError("Could not find ScreenIds script file! Create the file and try again.");
            }
        }

        private static void WriteIdClass(Dictionary<string, string> idPaths, string filePath) {
            var targetUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace(ScreenIdScriptNamespace);
            var targetClass = new CodeTypeDeclaration(ScreenIdScriptName) {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed
            };

            codeNamespace.Types.Add(targetClass);
            targetUnit.Namespaces.Add(codeNamespace);

            foreach (var idPathPair in idPaths) {
                var idField = new CodeMemberField(typeof(string), idPathPair.Key) {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    InitExpression = new CodePrimitiveExpression(idPathPair.Key)
                };

                targetClass.Members.Add(idField);
            }

            GenerateCSharpCode(targetUnit, filePath);
        }

        private static void GenerateCSharpCode(CodeCompileUnit targetUnit, string fileName) {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions();

            using (var sourceWriter = new StreamWriter(fileName)) {
                provider.GenerateCodeFromCompileUnit(targetUnit, sourceWriter, options);
            }
        }
    }
}
