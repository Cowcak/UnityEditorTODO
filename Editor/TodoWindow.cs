using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class TodoWindow : EditorWindow
    {
        [MenuItem("Window/Editor TODO")]
        private static void ShowWindow()
        {
            var window = GetWindow<TodoWindow>();
            window.titleContent = new GUIContent("Editor TODO");
            window.Show();
        }
        
        public string createAssetPath;

        TodoModel t;
        SerializedObject GetTarget;
        SerializedProperty ThisList;
        int ListSize;
        private bool listenerRegistered;
        private string ModelPath;

        private void OnEnable()
        {
            AssetDatabase.Refresh();
            LoadModel();            
            LoadView();
        }

        private void LoadModel()
        {
            t = TodoUtils.GetScriptableObject<TodoModel>();
            if (t != null)
            {
                GetTarget = new SerializedObject(t);
                ThisList = GetTarget.FindProperty("Todos"); // Find the List in our script and create a refrence of it
                if (!listenerRegistered)
                {
                    t.OnChange += () =>
                    {
                        GetTarget = new SerializedObject(t);
                        ThisList = GetTarget.FindProperty("Todos");
                        LoadView();
                    };
                    listenerRegistered = true;
                }
               
            }
            else
            {
                listenerRegistered = false;
            }
        }

        private void LoadView()
        {   
            rootVisualElement.Clear();
            var root = this.rootVisualElement;
            if (t == null)
            {
                var view = Resources.Load<VisualTreeAsset>("CreateModelDialog");
                view.CloneTree(root);
            
                BindButton("createButton", CreateModel);
                BindInput("filePath");
            }
            else
            {
                var view = Resources.Load<VisualTreeAsset>("TodoList");
                view.CloneTree(root);

                var styles = Resources.Load<StyleSheet>("TodoList");
                root.styleSheets.Add(styles);
                
                // Create some list of data, here simply numbers in interval [1, 1000]
                const int itemCount = 1000;
                var items = new List<Todo>(itemCount);
                for (int i = 0; i < ThisList.arraySize; i++)
                {
                    var property = ThisList.GetArrayElementAtIndex(i);
                    
                    var id = property.FindPropertyRelative("Id");
                    var todoMessage = property.FindPropertyRelative("Message");
                    var prefab = property.FindPropertyRelative("Prefab");
                    var scene = property.FindPropertyRelative("Scene");

                    items.Add(new Todo()
                    {
                        Id = id.intValue,
                        Message = todoMessage.stringValue,
                        Prefab = prefab.objectReferenceValue,
                        Scene = scene.objectReferenceValue
                    });
                }

                var item = Resources.Load<VisualTreeAsset>("TodoListItem");
                
                // The "makeItem" function will be called as needed
                // when the ListView needs more items to render
                Func<VisualElement> makeItem = () => item.CloneTree();
                
                // As the user scrolls through the list, the ListView object
                // will recycle elements created by the "makeItem"
                // and invoke the "bindItem" callback to associate
                // the element with the matching data item (specified as an index in the list)
                Action<VisualElement, int> bindItem = (e, i) =>
                {
                    e.Q<Label>("item-name").text = items[i].Message;
                    if (items[i].Scene != null)
                    {
                        e.Q<ObjectField>().value = items[i].Scene;
                    }
                    else
                    {
                        e.Q<ObjectField>().value = items[i].Prefab;
                    }
                };
                
                var listView = root.Q<ListView>();
                listView.makeItem = makeItem;
                listView.bindItem = bindItem;
                listView.itemsSource = items;
                listView.selectionType = SelectionType.Single;
                
                // Callback invoked when the user double clicks an item    
                listView.onItemChosen += obj => this.SelectObject((Todo)obj);
                
                // Callback invoked when the user changes the selection inside the ListView
                listView.onSelectionChanged += objects => Debug.Log(objects);
            }
        }

        private void OnFocus()
        {
            Repaint();
        }
        
        private void CreateModel()
        {
            var folderPath = "Assets";

            if (!string.IsNullOrEmpty(createAssetPath))
            {
                folderPath += "/";
                folderPath += createAssetPath;
                
                if (createAssetPath.Last() != '/')
                {
                    folderPath += "/";
                }
            }
            
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                var message = rootVisualElement.Q<Label>(className: "warningMessage");
                message.text = "Path not valid:" + folderPath;
                return;
            }
            
            var model = CreateInstance(typeof(TodoModel));
            
            AssetDatabase.CreateAsset(model, "Assets/"+createAssetPath + "TodoModel.asset");
            
            Repaint();
            LoadModel();
            LoadView();
        }
        
        void BindButton(string name, Action clickEvent)
        {
            var button = rootVisualElement.Q<Button>(name);

            if (button != null)
            {
                button.clickable.clicked += clickEvent;
            }
        }

        void BindInput(string name)
        {
            var input = rootVisualElement.Q<TextField>(name);
            input?.RegisterValueChangedCallback((evt => createAssetPath = evt.newValue));
        }
    }
}