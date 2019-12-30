using System;
using Object = UnityEngine.Object;

[Serializable]
public class Todo
{
    public int Id;
    public string Message;
    public Object Prefab;
    public Object Scene;
}
