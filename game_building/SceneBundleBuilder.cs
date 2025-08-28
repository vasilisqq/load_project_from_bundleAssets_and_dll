#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

public class SceneBundleBuilder
{
    [MenuItem("Build/Build Scene Bundle")]
    static void BuildSceneBundle()
    {
        // 1. Find scene
        string scenePath = FindScenePath("DogPatrul");
        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError("Scene not found!");
            return;
        }

        // 2. setting bundle
        AssetBundleBuild build = new AssetBundleBuild
        {
            assetBundleName = "gamemodule",
            assetNames = new[] { scenePath }
        };

        // 3. crreate folder
        string outputPath = "Assets/AssetBundles";
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // 4. build
        BuildPipeline.BuildAssetBundles(
    outputPath,
    new[] { build },
    BuildAssetBundleOptions.ForceRebuildAssetBundle |
    BuildAssetBundleOptions.UncompressedAssetBundle,
    EditorUserBuildSettings.activeBuildTarget
);

        Debug.Log($"Bundle built at: {outputPath}");
    }

    static string FindScenePath(string sceneName)
    {
        string[] guids = AssetDatabase.FindAssets($"t:Scene {sceneName}");
        if (guids.Length == 0) return null;

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        Debug.Log($"Found scene: {path}");
        return path;
    }
}
#endif
