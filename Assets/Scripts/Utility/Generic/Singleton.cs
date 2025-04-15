using System.Linq;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static bool _applicationIsQuitting = false;

    
    public static T Instance
    {
        get {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit. Won't create again.");
                return null;
            }

            lock (_lock)
            {
                if (_instance != null) return _instance;

                _instance = (T)FindObjectOfType(typeof(T));
                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogError("[Singleton] Multiple instances found!");
                }
                if(_instance != null) return _instance;
                    

                 // Try to load prefab from Resources/Prefabs/Managers/
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Managers/" + typeof(T).Name);
                if(prefab == null)
                {
                    throw  new System.Exception("[Singleton] The prefab loaded from Resources/Prefabs/Managers/" +
                                       typeof(T).Name + " is null. Please check the prefab path and name.");
                }

                GameObject prefabInstance = Instantiate(prefab);
                if (!prefabInstance.TryGetComponent<T>(out _instance))
                {
                    throw new System.Exception("[Singleton] The prefab loaded from Resources/Prefabs/Managers/" +
                                   typeof(T).Name + " does not have a component of type " + typeof(T) + ".");
                }
                return _instance;
            }

        }
    }

    protected virtual void OnDestroy()
    {
        //_applicationIsQuitting = true;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
