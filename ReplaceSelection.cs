using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ReplaceSelection : ScriptableWizard
{

	[SerializeField] private GameObject _prefab;
	[SerializeField] private GameObject[] _sceneObjects;

	[MenuItem("Tools/ReplaceObjects")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard(
			"Replace Selection", typeof(ReplaceSelection), "Replace");
	}


	void OnWizardCreate()
	{
		if(_prefab == null)
			return;
		
		if (_sceneObjects == null || _sceneObjects.Length < 1)
			return;
		
		foreach (GameObject sceneObject in _sceneObjects) {
			GameObject swapPrefab = (GameObject)EditorUtility.InstantiatePrefab(_prefab);
			swapPrefab.transform.parent = sceneObject.transform.parent;
			swapPrefab.transform.position = sceneObject.transform.position;
			DestroyImmediate (sceneObject);
		}

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}
}