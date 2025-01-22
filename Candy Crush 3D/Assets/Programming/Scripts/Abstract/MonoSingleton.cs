using UnityEngine;

/// <summary>
///     Handles singleton of type T
///     IMPORTANT: Make sure there is an instance of the type T in the same scene as from the scene the Instance is called
/// </summary>
/// <typeparam name="T">Type to create a singleton of</typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _Instance;
    private static readonly object _Lock = new object();

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                lock (_Lock)
                {
                    _Instance = FindObjectOfType<T>();

                    if (_Instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = $"{typeof(T).Name} (MonoSingleton)"
                        };
                        _Instance = obj.AddComponent<T>();
                        Debug.LogWarning($"Created new singleton instance for {typeof(T).Name}");
                    }
                }
            }
            return _Instance;
        }
    }

    protected virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
        }
    }
}