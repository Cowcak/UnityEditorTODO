using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TodoComponent))]
    public class TodoComponentEditor : UnityEditor.Editor
    {
        private TodoComponent _target;

        private void OnEnable()
        {
            _target = target as TodoComponent;
            
        }
        public override void OnInspectorGUI()
        {
            var message = serializedObject.FindProperty("Message");
            var id = serializedObject.FindProperty("Id");

            EditorGUILayout.SelectableLabel(id.intValue.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.PropertyField(message);

            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Save"))
            {
                _target.Save();
            }
            
            if (GUILayout.Button("Delete"))
            {
                _target.Remove();
                return;
            }
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}