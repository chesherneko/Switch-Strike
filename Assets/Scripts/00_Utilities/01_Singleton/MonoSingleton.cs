using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    return null;
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            return;
        }

        if (_instance != this as T)
            Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this as T)
            _instance = null;
    }
}