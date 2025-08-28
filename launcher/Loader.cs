using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Reflection;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [Header("Bundle Settings")]
    private string bundleName = "bundleName";
    private string sceneName = "sceneName";
    private string dllName = "dllName.dll";

    private AssetBundle _loadedBundle;
    private Assembly _loadedAssembly;

    private void Start()
    {
        StartCoroutine(PrepareData());
    }

    private IEnumerator PrepareData()
    {
        // using persistentDataPath for crossplatforming
        string bundlePath = Path.Combine(
            Application.persistentDataPath,
            bundleName
        );
        string dllPath = Path.Combine(Path.GetDirectoryName(bundlePath), dllName);
        //sequentially load the assembly and asset bundle
        yield return LoadAssemblyFromFile(dllPath);
        yield return LoadAssetBundle(bundlePath);
        if (_loadedBundle == null) yield break;
        yield return LoadSceneFromBundle(sceneName);
        yield return null;
    }

    private IEnumerator LoadAssemblyFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"DLL not found: {path}");
            yield break;
        }
        try
        {
            byte[] dllBytes = File.ReadAllBytes(path);
            _loadedAssembly = Assembly.Load(dllBytes);
            Debug.Log($"Assembly loaded: {_loadedAssembly.FullName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Assembly load failed: {ex}");
        }
    }

    private IEnumerator LoadAssetBundle(string path)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        _loadedBundle = request.assetBundle;
        if (_loadedBundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle at path: {path}");
        }
    }

    private IEnumerator LoadSceneFromBundle(string sceneName)
    {
        string[] scenePaths = _loadedBundle.GetAllScenePaths();
        if (scenePaths.Length == 0)
        {
            Debug.LogError("No scenes found in the asset bundle");
            yield break;
        }
        string targetScenePath = null;
        foreach (string path in scenePaths)
        {
            if (Path.GetFileNameWithoutExtension(path) == sceneName)
            {
                targetScenePath = path;
                break;
            }
        }
        if (string.IsNullOrEmpty(targetScenePath))
        {
            Debug.LogError($"Scene '{sceneName}' not found in asset bundle");
            yield break;
        }
        Scene launcherScene = SceneManager.GetActiveScene();
        var sceneLoadOperation = SceneManager.LoadSceneAsync(targetScenePath, LoadSceneMode.Additive);
        yield return sceneLoadOperation;

        if (sceneLoadOperation.isDone)
        {
            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);
            Debug.Log($"Scene '{sceneName}' set as active");
            yield return SceneManager.UnloadSceneAsync(launcherScene);
            Debug.Log("Launcher scene unloaded");
        }
    }

}
