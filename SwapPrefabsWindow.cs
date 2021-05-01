using UnityEditor;
using UnityEngine;



public class SwapPrefabsWindow : EditorWindow
{
    private GameObject[] _prefabs = new GameObject[0];
    private GameObject[] _prefabsSwap = new GameObject[0];
    
    private DefaultAsset _prefabsFolder = null;
    private DefaultAsset _prefabsSwapFolder = null;
    
    private bool _isShowedPrefabs;
    private bool _isShowedPrefabsSwap;
    
    private Vector2 _currentScrollPrefabs;
    private Vector2 _currentScrollPrefabsSwap;


    [MenuItem("Tools/Swap prefabs")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SwapPrefabsWindow window = (SwapPrefabsWindow)EditorWindow.GetWindow(typeof(SwapPrefabsWindow));
        window.Show();
    }

    void OnGUI()
    {
        // SELECT PREFABS
        GUILayout.Label("Select prefabs folder");
        _prefabsFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Select Folder", 
            _prefabsFolder, 
            typeof(DefaultAsset), 
            false);
 
        if (_prefabsFolder == null) 
            return;

        if (GUILayout.Button("Get All Prefabs"))
        {
            GetPrefabs(ref _prefabs, _prefabsFolder);
        }
        // END SELECT PREFABS
        
        if (_prefabs.Length < 1) return;
        
        // OUTPUT PREFABS
        _isShowedPrefabs = EditorGUILayout.Foldout(_isShowedPrefabs, $"Prefabs ({_prefabs.Length})");
        if (_isShowedPrefabs)
        {
            _currentScrollPrefabs = EditorGUILayout.BeginScrollView(_currentScrollPrefabs);
        
            for (int i = 0 ; i < _prefabs.Length; i++)
                EditorGUILayout.ObjectField(i.ToString(), _prefabs[i], typeof(GameObject));

            EditorGUILayout.EndScrollView();
        }
        //END OUTPUT PREFABS
        
        // MOBILE PREFABS
        
        // SELECT PREFABS
        GUILayout.Label("Select swap prefabs folder");
        _prefabsSwapFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Select Folder", 
            _prefabsSwapFolder, 
            typeof(DefaultAsset), 
            false);
 
        if (_prefabsSwapFolder == null) 
            return;

        if (GUILayout.Button("Get All Swap Prefabs"))
        {
            GetPrefabs(ref _prefabsSwap ,_prefabsSwapFolder);
        }
        // END SELECT PREFABS
        
        if (_prefabsSwap.Length < 1) return;
        
        // OUTPUT PREFABS
        _isShowedPrefabsSwap = EditorGUILayout.Foldout(_isShowedPrefabsSwap, $"Swap Prefabs ({_prefabsSwap.Length})");
        if (_isShowedPrefabsSwap)
        {
            _currentScrollPrefabsSwap = EditorGUILayout.BeginScrollView(_currentScrollPrefabsSwap);
        
            for (int i = 0 ; i < _prefabsSwap.Length; i++)
                EditorGUILayout.ObjectField(i.ToString(), _prefabsSwap[i], typeof(GameObject));

            EditorGUILayout.EndScrollView();
        }
        //END OUTPUT PREFABS
        
        // SWAP SECTION

        if (GUILayout.Button("Swap prefabs"))
        {
            Swap();
        }
    }

    private GameObject GetPrefabInCollection(ref GameObject[] prefabs, string name)
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.name == name)
                return prefab;
        }

        return null;
    }

    private void GetPrefabs(ref GameObject[] prefabs, DefaultAsset folder)
    {
        string path = AssetDatabase.GetAssetPath(folder);
        string[] assets = AssetDatabase.FindAssets("", new [] { path });
        
        prefabs = new GameObject[assets.Length];
       
        for (int i = 0 ; i < assets.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
            var gameObject = AssetDatabase.LoadAssetAtPath (assetPath, typeof(GameObject)) as GameObject;
            
            if (gameObject == null) continue;

            prefabs[i] = gameObject;
        }
    }
    
    private void Swap()
    {
        GameObject[] allGOs = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)); // ALL OBJECTS IN SCENE

        int foundedPrefabs = 0;
        int upadtedPrefabs = 0;
        
        foreach (var go in allGOs)
        {
            EditorUtility.DisplayProgressBar("Swap prefabs", $"Prefab {foundedPrefabs}/{allGOs.Length}", (float)foundedPrefabs / allGOs.Length);
            if (PrefabUtility.GetPrefabType(go) != PrefabType.PrefabInstance) continue;

            foundedPrefabs++;
            string parent = GetParentName(_prefabs, go);

            if (string.IsNullOrEmpty(parent))
            {
                Debug.Log($"NO PREFAB WAS FOUNDED: {go.name}");
                continue;
            }

            GameObject swapPrefab = GetPrefabByName(_prefabsSwap, parent);

            if (swapPrefab == null)
            {
                Debug.Log($"NO PREFAB WAS FOUNDED: {parent}");
                continue;
            }
            
            swapPrefab = (GameObject)EditorUtility.InstantiatePrefab(swapPrefab);

            swapPrefab.transform.parent = go.transform.parent;
            
            swapPrefab.transform.position = go.transform.position;
            swapPrefab.transform.rotation = go.transform.rotation;
            swapPrefab.transform.localScale = go.transform.localScale;
            
            DestroyImmediate(go);
            
            upadtedPrefabs++;
        }
        
        EditorUtility.ClearProgressBar();
        
        Debug.Log($"Was updated prefabs in scene: {upadtedPrefabs} ({foundedPrefabs})");
    }

    private string GetParentName(GameObject[] prefabs, GameObject go)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (PrefabUtility.GetPrefabParent(go) == _prefabs[i])
                return _prefabs[i].name;
        }

        return "";
    }

    private GameObject GetPrefabByName(GameObject[] prefabs, string name)
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].name == name)
                return prefabs[i];
        }

        return null;
    }
}
