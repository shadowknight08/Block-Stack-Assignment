using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component, IPoolable
{
    private readonly Queue<T> objects = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;
    private readonly int preloadCount;

    public ObjectPool(T prefab, int preloadCount = 0, Transform parent = null)
    {
        this.prefab = prefab;
        this.preloadCount = Mathf.Max(0, preloadCount);
        this.parent = parent;

        for (int i = 0; i < this.preloadCount; i++)
        {
            T obj = CreateInstance();
            obj.OnDespawned();
            objects.Enqueue(obj);
        }
    }

    private T CreateInstance()
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.SetReturnHandler(Return);
        return instance;
    }

    public T Get()
    {
        T instance = objects.Count > 0 ? objects.Dequeue() : CreateInstance();
        instance.OnSpawned();
        return instance;
    }

    public void Return(IPoolable pooled)
    {
        T obj = pooled as T;
        if (obj == null) return;
        obj.OnDespawned();
        objects.Enqueue(obj);
    }
}


