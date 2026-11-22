using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    // Subclasses must return right after base.Awake() when Instance != this: the object is a copy being destroyed.
    protected virtual void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;

        if (!transform.parent) {
            DontDestroyOnLoad(gameObject);
        }
    }
}
