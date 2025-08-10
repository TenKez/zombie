using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class FogOfWarSystem : NetworkBehaviour
{
    public int worldSize = 512;
    public int tileSize = 8; // meters per tile

    private int tiles => worldSize / tileSize;
    private Dictionary<ulong, HashSet<Vector2Int>> _discovered = new Dictionary<ulong, HashSet<Vector2Int>>();

    public void Reveal(ulong clientId, Vector3 position, float radius = 12f)
    {
        if (!IsServer) return;
        if (!_discovered.TryGetValue(clientId, out var set))
        {
            set = new HashSet<Vector2Int>();
            _discovered[clientId] = set;
        }
        int r = Mathf.CeilToInt(radius / tileSize);
        Vector2Int p = new Vector2Int(Mathf.FloorToInt(position.x / tileSize), Mathf.FloorToInt(position.z / tileSize));
        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
            set.Add(new Vector2Int(p.x + x, p.y + y));
    }

    public HashSet<Vector2Int> GetDiscovered(ulong clientId)
    {
        if (!_discovered.TryGetValue(clientId, out var set))
        {
            set = new HashSet<Vector2Int>();
            _discovered[clientId] = set;
        }
        return set;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShareWithTeamServerRpc(int teamId, ulong fromClientId)
    {
        // In a complete game, merge sets by team; here we merge everyone for simplicity.
        var src = GetDiscovered(fromClientId);
        foreach (var kv in _discovered)
            kv.Value.UnionWith(src);
    }
}
