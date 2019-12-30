using System;
using UnityEditor;
using UnityEngine;

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

        TodoModel t;
        SerializedObject GetTarget;
        SerializedProperty ThisList;
        int ListSize;

        private void OnEnable()
        {
            t = TodoUtils.GetScriptableObject<TodoModel>();
            GetTarget = new SerializedObject(t);
            ThisList = GetTarget.FindProperty("Todos"); // Find the List in our script and create a refrence of it
        }

        private void OnGUI()
        {
             //Update our list
       
            GetTarget.Update();
       
            //Choose how to display the list<> Example purposes only
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
       
            //Resize our list
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            ListSize = ThisList.arraySize;
            ListSize = EditorGUILayout.IntField ("List Size", ListSize);
       
            if(ListSize != ThisList.arraySize){
                while(ListSize > ThisList.arraySize){
                    ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
                }
                while(ListSize < ThisList.arraySize){
                    ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
                }
            }
       
            EditorGUILayout.Space ();

            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
       
            //Display our list to the inspector window

            int totalGames = 0;
            for(int i = 0; i < ThisList.arraySize; i++){
                SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
                SerializedProperty TodoMessage = MyListRef.FindPropertyRelative("Message");
                SerializedProperty Prefab = MyListRef.FindPropertyRelative("Prefab");
                SerializedProperty Scene = MyListRef.FindPropertyRelative("Scene");
                SerializedProperty Id = MyListRef.FindPropertyRelative("Id");
                
                EditorGUILayout.SelectableLabel(Id.intValue.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

                EditorGUILayout.SelectableLabel(TodoMessage.stringValue, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                
                if (Scene.objectReferenceValue == null)
                {   
                    EditorGUILayout.PropertyField(Prefab);
                }
                else
                {
                    EditorGUILayout.PropertyField(Scene);
                }

                EditorGUILayout.Space ();
           
                //Remove this index from the List
                if(GUILayout.Button("Select")){
                    this.SelectObject(i);
                }
                EditorGUILayout.Space ();
            }
       
            //Apply the changes to our list
            GetTarget.ApplyModifiedProperties();
        }
    }
}