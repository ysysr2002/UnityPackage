using UnityEngine;

//通过泛型获取单例的类型
public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; } = null;
    protected virtual void Awake()
    {
        Instance = this as T;
    }
}
