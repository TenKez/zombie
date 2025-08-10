using UnityEngine;
using Unity.Netcode;

public class BuildSystem : NetworkBehaviour
{
    public LayerMask groundMask;
    public GameObject[] buildPrefabs;
    public float maxHeight = 20f;

    [ServerRpc]
    public void TryPlaceServerRpc(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (prefabIndex < 0 || prefabIndex >= buildPrefabs.Length) return;
        if (position.y > maxHeight) return;

        // Simplified stability: must be close to any collider below within 2m
        if (!Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f, groundMask))
            return;

        var go = Instantiate(buildPrefabs[prefabIndex], position, rotation);
        go.GetComponent<NetworkObject>().Spawn(true);
    }
}
