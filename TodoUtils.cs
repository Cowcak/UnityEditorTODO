using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Object = System.Object;

public static class TodoUtils
{
    public static T GetScriptableObject<T>()
        where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string[]
            guids = AssetDatabase.FindAssets("t:" + typeof(T)
                                                 .Name); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a.FirstOrDefault();
#else
            return null;
#endif
    }

#if  UNITY_EDITOR
    
    public static void SelectObject(this ScriptableObject origin, Todo item)
    {
        if (item.Scene == null)
        {
            SelectPrefab(item);
        }
        else
        {
            SelectSceneObject(origin, item);
        }
    }

    private static void SelectPrefab(Todo todo)
    {
        var model = TodoUtils.GetScriptableObject<TodoModel>();
        var item = model.Todos.FirstOrDefault(x => x.Prefab == (Object)todo.Prefab);
        if (item != null)
        {
            // Load prefab
            Selection.activeGameObject = (GameObject)item.Prefab;
        }
    }

    public static void SelectSceneObject(ScriptableObject origin,  Todo item)
    {
        var sceneAsset = (SceneAsset)item.Scene;
        EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset));

        var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootObject in rootObjects)
        {
           var components = rootObject.GetComponentsInChildren<TodoComponent>();
           foreach (var component in components)
           {
               if (component.Id == item.Id)
               {
                   Selection.activeGameObject = component.gameObject;
                   return;
               }
           }
        }
    }

    public static void SaveScene()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }
    
#endif
}