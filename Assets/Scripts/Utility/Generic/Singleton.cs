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
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Multiple instances found!");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        // Try to load prefab from Resources/Prefabs/Managers/
                        GameObject prefab = Resources.Load<GameObject>("Prefabs/Managers/" + typeof(T).Name);
                        if (prefab != null)
                        {
                            GameObject prefabInstance = Instantiate(prefab);
                            _instance = prefabInstance.GetComponent<T>();
                            if (_instance == null)
                            {
                                Debug.LogError("[Singleton] The prefab loaded from Resources/Prefabs/Managers/" +
                                               typeof(T).Name + " does not have a component of type " + typeof(T) + ".");
                            }
                        }
                        else
                        {
                            GameObject singletonObj = new GameObject();
                            _instance = singletonObj.AddComponent<T>();
                            singletonObj.name = "(Singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singletonObj);
                            Debug.Log("[Singleton] A new instance of " + typeof(T) + " was created.");
                        }
                    }
                    return _instance;
                }

                return _instance;
            }

        }
    }

    protected virtual void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}
