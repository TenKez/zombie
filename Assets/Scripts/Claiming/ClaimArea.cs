using UnityEngine;
using Unity.Netcode;

public class ClaimArea : NetworkBehaviour
{
    public NetworkVariable<Vector3> CornerA = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> CornerB = new NetworkVariable<Vector3>();
    public NetworkVariable<ulong> ClaimOwnerClientId = new NetworkVariable<ulong>(0); // ← renamed

    public bool Contains(Vector3 pos)
    {
        var min = Vector3.Min(CornerA.Value, CornerB.Value);
        var max = Vector3.Max(CornerA.Value, CornerB.Value);
        return pos.x >= min.x && pos.x <= max.x && pos.z >= min.z && pos.z <= max.z;
    }

    public bool CanBuild(ulong clientId, Vector3 at) => ClaimOwnerClientId.Value == clientId && Contains(at);
}
