using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        // if an instance already exists and it's not this, destroy this duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;

        // Ensure DontDestroyOnLoad is applied to the root GameObject (required by Unity)
        var root = gameObject.transform.root != null ? gameObject.transform.root.gameObject : gameObject;
        DontDestroyOnLoad(root);
    }
}
