using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.Compilation;

public static class DLLExporter
{
    [MenuItem("Tools/Export Game DLL")]
    public static void ExportGameDLL()
    {
        // name of your build (same as name .asmdef)
        string assemblyName = "MyGameAssembly";

        // Path to save
        string outputPath = EditorUtility.SaveFolderPanel("Export DLL Location", "", "");
        if (string.IsNullOrEmpty(outputPath)) return;

        // find build
        foreach (var assembly in CompilationPipeline.GetAssemblies())
        {
            if (assembly.name == assemblyName)
            {
                string dllPath = assembly.outputPath;
                string dllName = Path.GetFileName(dllPath);
                string pdbPath = Path.ChangeExtension(dllPath, ".pdb");

                // copy files
                File.Copy(dllPath, Path.Combine(outputPath, dllName), true);

                if (File.Exists(pdbPath))
                {
                    File.Copy(pdbPath, Path.Combine(outputPath, Path.GetFileName(pdbPath)), true);
                }

                Debug.Log($"Successfully exported {dllName} to {outputPath}");
                AssetDatabase.Refresh();
                return;
            }
        }

        Debug.LogError("Assembly not found! Check your asmdef name.");
    }
}
