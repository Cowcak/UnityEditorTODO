using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class TodoComponent : MonoBehaviour
{
    public string Message;

    public int Id;
    
    private void OnEnable()
    {
#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

#if UNITY_EDITOR
       
    public void Remove()
    {
        var model = TodoUtils.GetScriptableObject<TodoModel>();
        model.RemoveTodo(Id);
        
        DestroyImmediate(this);

    }

    [ContextMenu("Save")]
    public void Save()
    {
        var mainStage = StageUtility.GetMainStageHandle();
        var currentStage = StageUtility.GetStageHandle(gameObject);

        if (mainStage == currentStage)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                // Prefab instance
                var scene = SceneManager.GetActiveScene();
                UpdateChanges(gameObject, AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)));
            }
            else
            {
                // Normal object in scene
                var scene = SceneManager.GetActiveScene();
                UpdateChanges(gameObject, AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)));
            }
        }
        else
        {
            var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null)
            {
                UpdateChanges(AssetDatabase.LoadAssetAtPath(prefabStage.prefabAssetPath, typeof(GameObject)));
            }
        }
    }

    [ContextMenu("Select")]
    public void Select()
    {        
        var model = TodoUtils.GetScriptableObject<TodoModel>();
        var item = model.Todos.FirstOrDefault(x => x.Prefab == (Object)gameObject);
        if (item != null)
        {
            Selection.activeGameObject = (GameObject)item.Prefab;
        }

    }
    
    [ContextMenu("Dump")]
    public void Dump()
    {
        Debug.Log(Id);
    }

    private void UpdateChanges(Object source, Object sceneAsset = null)
    {
        var model = TodoUtils.GetScriptableObject<TodoModel>();
        if (Id == 0)
        {
            Id = model.AddTodo(Message,source, sceneAsset);
        }
        else if (string.IsNullOrEmpty(Message))
        {
            // Remove todo
            model.RemoveTodo(Id);
        }
        else
        {
            // Update todo
            model.UpdateTodo(Id, Message);
        }
        
        TodoUtils.SaveScene();
    }
#endif
 
}