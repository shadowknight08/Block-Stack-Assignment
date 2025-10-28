using System;
using UnityEngine;

public interface IPoolable
{
    // Called by pool when the object is handed out
    void OnSpawned();

    // Called by pool when the object is returned
    void OnDespawned();

    // Pool injects a handler the object can use to request a return
    void SetReturnHandler(Action<IPoolable> handler);

    // Exposed so external systems (e.g., PoolManager) can force a return
    Action<IPoolable> GetReturnHandler();

    // For positioning after spawn
    Transform transform { get; }
}


