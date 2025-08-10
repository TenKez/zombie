using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public struct ScentNode : INetworkSerializable
{
    public Vector3 Position;
    public float CreatedAt;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref CreatedAt);
    }
}

public class ScentTrailEmitter : NetworkBehaviour
{
    public float dropInterval = 1f;
    public float trailLifetime = 60f; // 1 minute

    private float _timer;
    private static List<ScentNode> _nodes = new List<ScentNode>();

    private void Update()
    {
        if (!IsServer) return;
        _timer += Time.deltaTime;
        if (_timer >= dropInterval)
        {
            _timer = 0f;
            _nodes.Add(new ScentNode { Position = transform.position, CreatedAt = Time.time });
        }

        // Cleanup old nodes
        _nodes.RemoveAll(n => Time.time - n.CreatedAt > trailLifetime);
    }

    public static Vector3? GetRecentDirection(Vector3 from, float withinSeconds = 60f)
    {
        ScentNode? closest = null;
        float best = float.MaxValue;
        for (int i = _nodes.Count - 1; i >= 0; i--)
        {
            var n = _nodes[i];
            float age = Time.time - n.CreatedAt;
            if (age > withinSeconds) continue;
            float d = (n.Position - from).sqrMagnitude;
            if (d < best)
            {
                best = d;
                closest = n;
            }
        }
        if (closest.HasValue) return closest.Value.Position;
        return null;
    }
}
