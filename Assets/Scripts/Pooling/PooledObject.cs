using System;
using UnityEngine;

// Backwards-compatible base component implementing IPoolable.
public class PooledObject : MonoBehaviour, IPoolable
{
    private Action<IPoolable> returnHandler;

    public virtual void OnSpawned()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDespawned()
    {
        gameObject.SetActive(false);
    }

    public void SetReturnHandler(Action<IPoolable> handler)
    {
        returnHandler = handler;
    }

    public Action<IPoolable> GetReturnHandler()
    {
        return returnHandler;
    }

    protected void ReturnToPool()
    {
        returnHandler?.Invoke(this);
    }
}


