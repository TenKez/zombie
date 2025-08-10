using UnityEngine;
using Unity.Netcode;

public class ClaimFlagCorner : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void PlaceServerRpc(Vector3 worldPos)
    {
        transform.position = worldPos;
    }
}
