using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu]
public class TodoModel : ScriptableObject
{
        public int LastId;
        public List<Todo> Todos = new List<Todo>();

        private void OnEnable()
        {
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

                return id;
        }

        public void UpdateTodo(int guid, string message)
        {
                var item = Todos.FirstOrDefault(x => x.Id == guid);
                if (item != null)
                {
                        item.Message = message;
                }
        }

        public void RemoveTodo(int guid)
        {
                var item = Todos.FirstOrDefault(x => x.Id == guid);
                if (item != null)
                {
                        Todos.Remove(item);
                }
        }

        [ContextMenu("Dump")]
        public void Dump()
        {
                Todos.ForEach(x => Debug.Log(x.Id));
        }
}