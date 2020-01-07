using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorTODO;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[CreateAssetMenu]
public class TodoModel : ScriptableObject
{
        public int LastId;
        public List<Todo> Todos = new List<Todo>();

        public UnityAction OnChange;

        private void OnEnable()
        {
                OnChange = () => { };
        }

        public int AddTodo(string message, Object go, Object scene)
        {
                LastId++;
                var id = LastId  ;
                var newTodo = new Todo()
                {
                        Id = id,
                        Message = message, 
                        Prefab = go,
                        Scene = scene
                };
                
                Todos.Add(newTodo);

                OnChange?.Invoke();
                
                return id;
        }

        public void UpdateTodo(int guid, string message)
        {
                var item = Todos.FirstOrDefault(x => x.Id == guid);
                if (item != null)
                {
                        item.Message = message;
                        OnChange.Invoke();
                }
        }

        public void RemoveTodo(int guid)
        {
                var item = Todos.FirstOrDefault(x => x.Id == guid);
                if (item != null)
                {
                        Todos.Remove(item);
                        OnChange.Invoke();
                }
        }
}