using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public struct NoiseEvent
{
    public Vector3 Position;
    public float Strength;
}

public class NoiseSystem : NetworkBehaviour
{
    public static NoiseSystem Instance;

    private readonly List<NoiseEvent> _events = new List<NoiseEvent>();
    private readonly List<NoiseEvent> _snapshot = new List<NoiseEvent>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) enabled = false; // server-authoritative
    }

    public static void Emit(Vector3 pos, float strength)
    {
        if (Instance == null || !Instance.IsServer) return;
        Instance._events.Add(new NoiseEvent { Position = pos, Strength = strength });
    }

    // Zombies read everything produced this frame (non-destructive).
    public IReadOnlyList<NoiseEvent> GetSnapshot()
    {
        _snapshot.Clear();
        _snapshot.AddRange(_events);
        return _snapshot;
    }

    // Clear once per frame after all AIs have read it.
    private void LateUpdate()
    {
        if (!IsServer) return;
        _events.Clear();
    }
}
