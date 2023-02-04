using UnityEngine;

// Permite volver a crear el objeto de 0 pero funciona como un Singleton
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

// Singleton se elimina al cambiar de escena
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
        else
            base.Awake();
    }
}

// SingletonPersistent se guarda entre Escenas
public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(Instance);
        //PersistentParentsRecursive(Instance.gameObject);
    }

    private static void PersistentParentsRecursive(GameObject obj)
    {
        if (obj.transform.parent != null)
        {
            PersistentParentsRecursive(obj.transform.parent.gameObject);
            DontDestroyOnLoad(obj.transform.parent.gameObject);
        }
    }
}