using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, object> prefabToPool = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, int preload = 0) where T : Component, IPoolable
    {
        ObjectPool<T> pool = GetOrCreatePool(prefab, preload);
        T obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void Despawn(IPoolable obj)
    {
        if (obj == null) return;
        // Route back via handler
        obj.GetReturnHandler()?.Invoke(obj);
    }

    private ObjectPool<T> GetOrCreatePool<T>(T prefab, int preload) where T : Component, IPoolable
    {
        if (!prefabToPool.TryGetValue(prefab.gameObject, out var boxedPool))
        {
            var pool = new ObjectPool<T>(prefab, preload, transform);
            prefabToPool[prefab.gameObject] = pool;
            return pool;
        }
        return (ObjectPool<T>)boxedPool;
    }
}


