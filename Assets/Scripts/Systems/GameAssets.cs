using System.Diagnostics;
using UnityEngine;

public static class GameAssets
{
    public static GameObject playerPrefab;
    public static Controls controls;

    public static bool isInitialized { get; private set; } = false;

    public static void Init()
    {
        if (!isInitialized)
        {
            Stopwatch resLoad = Stopwatch.StartNew();

            playerPrefab = Resources.Load<GameObject>("Player/Player");
            controls = null;
            controls = new Controls();

            isInitialized = true;
            resLoad.Stop();
            UnityEngine.Debug.Log($"Resources loaded. Loading time: {resLoad.ElapsedMilliseconds}ms");
        }
        else{
            UnityEngine.Debug.Log("Assets already initialized.");
        }
    }
}
